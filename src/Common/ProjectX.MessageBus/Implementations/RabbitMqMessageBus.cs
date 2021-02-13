using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectX.Core;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.Threading;
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
        #region Private members

        private readonly ReaderWriterLockSlim _syncRootForSubscribers = new ReaderWriterLockSlim();
        private readonly Dictionary<SubscriptionKey, SubscriberInfo> _subscribers = new Dictionary<SubscriptionKey, SubscriberInfo>();

        private readonly ReaderWriterLockSlim _syncRootForPublishers = new ReaderWriterLockSlim();
        private readonly Dictionary<SubscriptionKey, IModel> _publishers = new Dictionary<SubscriptionKey, IModel>();

        private readonly IRabbitMqConnectionService _connectionService;
        private readonly ILogger<RabbitMqEventBus> _logger;

        private readonly IIntegrationEventSerializer _serializer;
        private readonly MessageBusOptions _messageBusOptions;

        #endregion

        #region Constructors

        public RabbitMqEventBus(IRabbitMqConnectionService connectionService,
            IIntegrationEventSerializer serializer,
            ILogger<RabbitMqEventBus> logger,
            IOptions<MessageBusOptions> options)
        {
            _connectionService = connectionService;
            _serializer = serializer;
            _logger = logger;
            _messageBusOptions = options.Value;
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
                        
        private static string GetRoutingKey<T>() => typeof(T).Name;

        private static string GetQueueName(string connectionName, string exchange, string routingKey) 
            => $"{connectionName}/{exchange}.{routingKey}";

        private static void Validate(ExchangeOptions exchange) 
        {
            Utill.ThrowIfNull(exchange, nameof(exchange));
            Utill.ThrowIfNull(exchange.Name, nameof(exchange.Name));
            Utill.ThrowIfNull(exchange.Type, nameof(exchange.Type));
        }

        public void Subscribe<T, TH>(RabbitMqEventBusProperties properties)
            where T : IIntegrationEvent
        {
            Utill.ThrowIfNull(properties, nameof(properties));
            Validate(properties.Exchange);

            if (string.IsNullOrEmpty(properties.Queue.RoutingKey))
                properties.Queue.RoutingKey = GetRoutingKey<T>();

            if (string.IsNullOrEmpty(properties.Queue.Name))
                properties.Queue.Name = GetQueueName(_messageBusOptions.ConnectionName, properties.Exchange.Name, properties.Queue.RoutingKey);

            var key = GetSubscriptionKey(properties.Exchange.Name, properties.Queue.RoutingKey);

            using (new ReadLock(_syncRootForSubscribers)) 
            {
                if (_subscribers.ContainsKey(key)) 
                {
                    _logger.LogError($"Integration event of type {typeof(T).Name} already subscribed for '{key}'.");
                    return;
                }
            }

            var subscriptionInfo = CreateSubscriberChannel<T>(properties);

            using (new WriteLock(_syncRootForSubscribers)) 
            {
                if (_subscribers.ContainsKey(key))
                {
                    //TO DO: consider about this.
                    subscriptionInfo.Channel.Dispose();
                    return;
                }
                else 
                {
                    _subscribers.Add(key, subscriptionInfo);
                }
            }

            _logger.LogInformation($"New subscription: {subscriptionInfo}");
        }

        public void Unsubscribe<T>(RabbitMqEventBusProperties properties)
            where T : IIntegrationEvent
        {
            Utill.ThrowIfNull(properties, nameof(properties));
            Validate(properties.Exchange);

            var exchangeName = properties.Exchange.Name;
            var routingKey = properties.Queue?.RoutingKey ?? GetRoutingKey<T>();

            var key = GetSubscriptionKey(exchangeName, routingKey);

            SubscriberInfo subscriptionInfo = null;
            
            bool exists = false;

            using (new ReadLock(_syncRootForSubscribers)) 
            {
                exists = _subscribers.TryGetValue(key, out subscriptionInfo);
            }

            if (!exists) 
            {
                _logger.LogError($"Subscriber: {key} was not found.");
                return;
            }

            if (subscriptionInfo.Channel != null)
            {
                if (!_connectionService.IsConnected)
                     _connectionService.TryConnect();

                if (!subscriptionInfo.Channel.IsClosed)
                {
                    subscriptionInfo.Channel.QueueUnbind(queue: subscriptionInfo.Queue.Name, exchange: exchangeName, routingKey: routingKey);
                    subscriptionInfo.Channel.Close();
                    subscriptionInfo.Channel.Dispose();
                }
            }

            using (new WriteLock(_syncRootForSubscribers)) 
            {
                _subscribers.Remove(key);
            }
        }

        #endregion

        #region IDisposable members

        public void Dispose()
        {
            using (new WriteLock(_syncRootForSubscribers)) 
            {
                _subscribers.Clear();
            }

            using (new WriteLock(_syncRootForPublishers))
            {
                _publishers.Clear();
            }
        }

        #endregion

        #region Private methods

        private void Publish(IModel channel, string exchange, string routingKey, IBasicProperties basicProperties, IIntegrationEvent integrationEvent)
        {
            byte[] body = _serializer.SerializeToBytes(integrationEvent);

            channel.BasicPublish(exchange, routingKey, basicProperties, body);
        }

        private SubscriberInfo CreateSubscriberChannel<T>(RabbitMqEventBusProperties properties)
            where T : IIntegrationEvent
        {
            if (!_connectionService.IsConnected && !_connectionService.TryConnect())
                 throw new Exception("Can't connect to RabbitMq.");

            var channel = _connectionService.CreateChannel();

            channel.ExchangeDeclare(exchange: properties.Exchange.Name, type: properties.Exchange.Type, durable: properties.Exchange.Durable, autoDelete: properties.Exchange.AutoDelete);

            var queueName = channel.QueueDeclare(queue: properties.Queue.Name, durable: properties.Queue.Durable, exclusive: properties.Queue.Exclusive, autoDelete: properties.Queue.AutoDelete).QueueName;

            channel.QueueBind(queue: queueName, exchange: properties.Exchange.Name, routingKey: properties.Queue.RoutingKey);

            channel.BasicQos(0, properties.Consumer.PrefetchCount, false);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += OnMessageReceived;

            channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            channel.CallbackException += (sender, ea) =>
            {
                _logger.LogError(ea.Exception, ea.Exception.Message);

                channel.Dispose();

                CreateSubscriberChannel<T>(properties);
            };

            return new SubscriberInfo(eventType: typeof(T), channel, properties.Queue, properties.Exchange, properties.Consumer);
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

            using (new ReadLock(_syncRootForSubscribers)) 
            {
                isExist = _subscribers.TryGetValue(key, out subscriptionInfo);
            }

            if (!isExist) 
            {
                _logger.LogError($"Subscriber: {key} was not found.");
                return;
            }

            try
            {
                IIntegrationEvent integrationEvent = (IIntegrationEvent)_serializer.Deserialize(message, subscriptionInfo.EventType);
               
                //await subscriptionInfo.EventHandler.HandleAsync(integrationEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }

        private SubscriptionKey GetSubscriptionKey(string exchange, string routingKey)
        {
            return new SubscriptionKey(exchange, routingKey);
        }

        #endregion
    }
}
