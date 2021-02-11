using Microsoft.Extensions.Logging;
using ProjectX.Core.IntegrationEvents;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Implementations
{
    public sealed class RabbitMqEventBus : IEventBus
    {
        #region Inner types

        /// <summary>
        /// Uses for for subscribers and publishers collections
        /// </summary>
        private struct SubscriptionKey
        {
            private readonly string _exchange;
            private readonly string _routingKey;

            public SubscriptionKey(string exchange, string routingKey)
            {
                _exchange = exchange;
                _routingKey = routingKey;
            }

            public override string ToString()
            {
                return $"{_exchange}.{_routingKey}";
            }

            public override int GetHashCode()
            {
                return (_exchange, _routingKey).GetHashCode();
            }
        }

        /// <summary>
        /// Uses for handling subscriptions for different message types
        /// </summary>
        private class SubscriberInfo
        {
            /// <summary>
            /// Type of message for handling
            /// </summary>
            public Type EventType { get; }

            public bool IsBinary { get; }

            /// <summary>
            /// Handler for specified message type
            /// </summary>
            //public Type EventHandlerType { get; }

            public object EventHandler { get; }

            /// <summary>
            /// Channel for subscription
            /// </summary>
            public IModel Channel { get; private set; }

            /// <summary>
            /// RabbitMQ queue name
            /// </summary>
            public string QueueName { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="eventType">Type of message for subscription</param>
            /// <param name="eventHandlerType">Message handler.</param>
            public SubscriberInfo(Type eventType, object handler, bool isBinary)
            {
                EventType = eventType;
                EventHandler = handler;
                IsBinary = isBinary;
            }

            /// <summary>
            /// Adds RabbitMQ channel and RabbitMQ queue name to subscription info
            /// </summary>
            /// <param name="channel">RabbitMQ channel</param>
            /// <param name="queueName">RabbitMQ queue name</param>
            public void AddChannel(IModel channel, string queueName)
            {
                if (string.IsNullOrEmpty(queueName))
                    throw new ArgumentNullException(nameof(queueName));

                Channel = channel ?? throw new ArgumentNullException(nameof(channel));
                QueueName = queueName;
            }
        }

        #endregion

        #region Private members

        private const string DirectExchangeType = "direct";
        private const string FanoutExchangeType = "fanout";

        private readonly ReaderWriterLockSlim _syncRootForSubscribers = new ReaderWriterLockSlim();
        private readonly Dictionary<SubscriptionKey, SubscriberInfo> _subscribers = new Dictionary<SubscriptionKey, SubscriberInfo>();

        private readonly ReaderWriterLockSlim _syncRootForPublishers = new ReaderWriterLockSlim();
        private readonly Dictionary<SubscriptionKey, IModel> _publishers = new Dictionary<SubscriptionKey, IModel>();

        private readonly IRabbitMqConnectionService _connectionService;
        private readonly ILogger _logger;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        #endregion

        #region Constructors

        public RabbitMqEventBus(IRabbitMqConnectionService connectionService,
            JsonSerializerOptions jsonSerializerOptions,
            ILogger<RabbitMqEventBus> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionService = connectionService ?? throw new ArgumentNullException(nameof(connectionService));

            _jsonSerializerOptions = jsonSerializerOptions;
            //_jsonSerializerOptions.Converters.Add(new RealtimeMessageConverter());
        }

        #endregion

        #region IRabbitMqEventBus members

        private IModel InitPublisher(RabbitMqEventBusProperties properties)
        {
            var key = GetSubscriptionKey(properties.Exchange, properties.RoutingKey);
            _syncRootForPublishers.EnterReadLock();
            try
            {
                if (_publishers.TryGetValue(key, out IModel channel))
                    return channel;
            }
            finally
            {
                _syncRootForPublishers.ExitReadLock();
            }

            var newChannel = _connectionService.CreateChannel();
            newChannel.ExchangeDeclare(exchange: properties.Exchange, type: properties.IsDirectExchange ? DirectExchangeType : FanoutExchangeType);

            _syncRootForPublishers.EnterWriteLock();
            try
            {
                if (_publishers.TryGetValue(key, out IModel existedChannel))
                {
                    if (!newChannel.IsClosed)
                    {
                        newChannel.Close();
                        newChannel.Dispose();
                    }

                    return existedChannel;
                }
                else
                {
                    _publishers.Add(key, newChannel);
                    _logger.LogInformation($"Added publisher to exchange {properties.Exchange.Value} with routing_key {properties.RoutingKey}.");
                    return newChannel;
                }
            }
            finally
            {
                _syncRootForPublishers.ExitWriteLock();
            }
        }

        public void AddPublisher(IEventBusProperties properties)
        {
            if (!(properties is RabbitMqEventBusProperties rabbitMqProperties))
                throw new ArgumentException($"Invalid type for {nameof(properties)}");

            var exchange = rabbitMqProperties.Exchange.Value;
            var routingKey = rabbitMqProperties.RoutingKey;

            if (!_connectionService.IsConnected)
                _connectionService.TryConnect();

            InitPublisher(rabbitMqProperties);
        }

        public bool RemovePublisher(IEventBusProperties properties)
        {
            if (!(properties is RabbitMqEventBusProperties rabbitMqProperties))
                throw new ArgumentException($"Invalid type for {nameof(properties)}");

            var key = GetSubscriptionKey(rabbitMqProperties.Exchange, rabbitMqProperties.RoutingKey);

            IModel channel;
            var result = false;

            _syncRootForPublishers.EnterWriteLock();
            try
            {
                if (_publishers.TryGetValue(key, out channel))
                    result = _publishers.Remove(key);
            }
            finally
            {
                _syncRootForPublishers.ExitWriteLock();
            }

            if (result)
            {
                lock (channel)
                {
                    channel.Close();
                    channel.Dispose();
                }

                _logger.LogInformation($"Removed publisher to exchange {rabbitMqProperties.Exchange.Value} with routing_key {rabbitMqProperties.RoutingKey}.");
            }

            return result;
        }

        public void Publish(IIntegrationEvent integrationEvent, IEventBusProperties properties)
        {
            if (!(properties is RabbitMqEventBusProperties rabbitMqProperties))
                throw new ArgumentException($"Invalid type for {nameof(properties)}");

            var exchange = rabbitMqProperties.Exchange;
            if (string.IsNullOrEmpty(exchange))
                throw new ArgumentException($"Parameter {nameof(exchange)} can't be null or empty.");

            var routingKey = rabbitMqProperties.RoutingKey;
            if (rabbitMqProperties.IsDirectExchange && string.IsNullOrEmpty(routingKey))
                throw new ArgumentException($"Parameter {nameof(routingKey)} can't be null or empty for Direct exchange type.");

            if (!_connectionService.IsConnected)
                _connectionService.TryConnect();

            IModel channel = InitPublisher(rabbitMqProperties);
            Publish(channel, exchange, routingKey, null, integrationEvent);
        }

        public void Subscribe<T, TH>(IEventBusProperties properties, TH handler)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            if (!(properties is RabbitMqEventBusProperties rabbitMqProperties))
                throw new ArgumentException($"Invalid type for {nameof(properties)}");

            var exchange = rabbitMqProperties.Exchange;
            if (string.IsNullOrEmpty(exchange))
                throw new ArgumentException($"Parameter {nameof(exchange)} can't be null or empty.");

            var routingKey = rabbitMqProperties.RoutingKey;
            if (rabbitMqProperties.IsDirectExchange && string.IsNullOrEmpty(routingKey))
                throw new ArgumentException($"Parameter {nameof(routingKey)} can't be null or empty for Direct exchange type.");

            var key = GetSubscriptionKey(exchange, routingKey);
            var isExist = false;

            _syncRootForSubscribers.EnterReadLock();
            try
            {
                isExist = _subscribers.ContainsKey(key);
            }
            finally
            {
                _syncRootForSubscribers.ExitReadLock();
            }

            if (isExist)
            {
                _logger.LogError($"Integration event of type {typeof(T).Name} already subscribed for '{key}'.");
            }
            else
            {
                var subscriptionInfo = new SubscriberInfo(typeof(T), handler, rabbitMqProperties.IsBinary);

                CreateConsumerChannel(exchange, routingKey, subscriptionInfo, rabbitMqProperties.IsDirectExchange);

                _syncRootForSubscribers.EnterWriteLock();
                try
                {
                    _subscribers.Add(key, subscriptionInfo);
                }
                finally
                {
                    _syncRootForSubscribers.ExitWriteLock();
                }

                _logger.LogInformation($"Subscribed new integration event of type {typeof(T).Name} to exchange {exchange} with routing_key {routingKey}.");
            }
        }

        public void Unsubscribe<T, TH>(IEventBusProperties properties)
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            if (!(properties is RabbitMqEventBusProperties rabbitMqProperties))
                throw new ArgumentException($"Invalid type for {nameof(properties)}");

            var exchange = rabbitMqProperties.Exchange;
            var routingKey = rabbitMqProperties.RoutingKey;

            if (string.IsNullOrEmpty(exchange) || string.IsNullOrEmpty(routingKey))
                throw new ArgumentException($"Parameters {nameof(exchange)} and {nameof(routingKey)} can't be null or empty.");

            var key = GetSubscriptionKey(exchange, routingKey);

            SubscriberInfo subscriptionInfo = null;
            var isExist = false;

            _syncRootForSubscribers.EnterReadLock();
            try
            {
                isExist = _subscribers.TryGetValue(key, out subscriptionInfo);
            }
            finally
            {
                _syncRootForSubscribers.ExitReadLock();
            }

            if (isExist)
            {
#warning check an order of removing and closing
                if (subscriptionInfo.Channel != null)
                    RemoveConsumerChannel(subscriptionInfo.Channel, subscriptionInfo.QueueName, exchange, routingKey);

                _syncRootForSubscribers.EnterWriteLock();
                try
                {
                    _subscribers.Remove(key);
                }
                finally
                {
                    _syncRootForSubscribers.ExitWriteLock();
                }
            }
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            _syncRootForSubscribers.EnterWriteLock();
            try
            {
                _subscribers.Clear();
            }
            finally
            {
                _syncRootForSubscribers.ExitWriteLock();
            }

            _syncRootForPublishers.EnterWriteLock();
            try
            {
                _publishers.Clear();
            }
            finally
            {
                _syncRootForPublishers.ExitWriteLock();
            }
        }

        #endregion

        #region Private methods

        private void Publish(IModel channel, string exchange, string routingKey, IBasicProperties basicProperties, IIntegrationEvent integrationEvent)
        {
            byte[] body = JsonSerializer.SerializeToUtf8Bytes(integrationEvent, integrationEvent.GetType(), _jsonSerializerOptions);

            channel.BasicPublish(exchange, routingKey, basicProperties, body);
        }

        private void CreateConsumerChannel(string exchange, string routingKey, SubscriberInfo subscriptionInfo, bool isDirectExchange)
        {
            if (!_connectionService.IsConnected)
                _connectionService.TryConnect(); //todo add validation

            var channel = _connectionService.CreateChannel();

            channel.ExchangeDeclare(exchange: exchange, type: isDirectExchange ? DirectExchangeType : FanoutExchangeType);

            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routingKey);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += OnMessageReceived;

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogError(ea.Exception, ea.Exception.Message);

                channel.Dispose();
                CreateConsumerChannel(exchange, routingKey, subscriptionInfo, isDirectExchange);
            };

            subscriptionInfo.AddChannel(channel, queueName);
        }

        private void RemoveConsumerChannel(IModel channel, string queueName, string exchange, string routingKey)
        {
            if (!_connectionService.IsConnected)
                _connectionService.TryConnect(); //todo add validation

            if (!channel.IsClosed)
            {
                channel.QueueUnbind(queue: queueName, exchange: exchange, routingKey: routingKey);

                channel.Close();
                channel.Dispose();
            }
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            var key = GetSubscriptionKey(ea.Exchange, ea.RoutingKey);
            await ProcessMessage(key, ea.Body.ToArray());
        }

        private async Task ProcessMessage(SubscriptionKey key, byte[] message)
        {
            SubscriberInfo subscriptionInfo = null;
            var isExist = false;

            _syncRootForSubscribers.EnterReadLock();
            try
            {
                isExist = _subscribers.TryGetValue(key, out subscriptionInfo);
            }
            finally
            {
                _syncRootForSubscribers.ExitReadLock();
            }

            if (isExist)
            {
                try
                {
                    IIntegrationEvent integrationEvent = null;

                    if (subscriptionInfo.IsBinary)
                    {
                        integrationEvent = new BinaryIntegrationEvent(message);
                    }
                    else
                    {
                        integrationEvent = (IIntegrationEvent)JsonSerializer.Deserialize(message, subscriptionInfo.EventType, _jsonSerializerOptions);
                    }

                    //await subscriptionInfo.EventHandler.HandleAsync(integrationEvent);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                }
            }
        }

        private SubscriptionKey GetSubscriptionKey(string exchange, string routingKey)
        {
            return new SubscriptionKey(exchange, routingKey);
        }

        #endregion
    }
}
