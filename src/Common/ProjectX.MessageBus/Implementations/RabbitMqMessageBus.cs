using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectX.Core;
using ProjectX.Core.Exceptions;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Implementations
{
    public sealed class RabbitMqMessageBus : IMessageBus
    {
        #region Private members

        private readonly ReaderWriterLockSlim _syncRootForSubscribers = new ReaderWriterLockSlim();
        private readonly Dictionary<SubscriptionKey, Subscriber> _subscribers = new Dictionary<SubscriptionKey, Subscriber>();

        private readonly ReaderWriterLockSlim _syncRootForPublishers = new ReaderWriterLockSlim();
        private readonly Dictionary<SubscriptionKey, Publisher> _publishers = new Dictionary<SubscriptionKey, Publisher>();

        private readonly IRabbitMqConnectionService _connectionService;
        private readonly ILogger<RabbitMqMessageBus> _logger;

        private readonly IMessageSerializer _serializer;
        private readonly MessageBusOptions _messageBusOptions;
        private readonly IMessageDispatcher _dispatcher;
        #endregion

        #region Constructors

        public RabbitMqMessageBus(IRabbitMqConnectionService connectionService,
            IMessageDispatcher dispatcher,
            IMessageSerializer serializer,
            ILogger<RabbitMqMessageBus> logger,
            IOptions<MessageBusOptions> options)
        {
            _connectionService = connectionService;
            _dispatcher = dispatcher;
            _serializer = serializer;
            _logger = logger;
            _messageBusOptions = options.Value;
        }

        #endregion

        #region IRabbitMqEventBus members

        public void AddPublisher(PublishOptions properties)
        {
            Utill.ThrowIfNull(properties, nameof(properties));
            Validate(properties.Exchange);
            InitPublisher(properties);
        }

        public bool RemovePublisher(PublishOptions properties)
        {
            Utill.ThrowIfNull(properties, nameof(properties));
            Utill.ThrowIfNullOrEmpty(properties.RoutingKey, nameof(properties.RoutingKey));
            Validate(properties.Exchange);

            var key = GetSubscriptionKey(properties.Exchange.Name, properties.RoutingKey);

            Publisher publisher = null;
            var result = false;

            using (new WriteLock(_syncRootForPublishers)) 
            {
                if (_publishers.TryGetValue(key, out publisher))
                    result = _publishers.Remove(key);
                else
                    return true;
            }

            if (result)
            {
                publisher.Close();

                _logger.LogInformation($"Removed publisher:{publisher}");
            }

            return result;
        }

        public void Publish<T>(T integrationEvent, PublishOptions properties)
            where T : IIntegrationEvent
        {
            Utill.ThrowIfNull(properties, nameof(properties));
            Validate(properties.Exchange);
            if (properties.Exchange.IsFanout && string.IsNullOrEmpty(properties.RoutingKey))
                properties.RoutingKey = GetRoutingKey<T>();

            byte[] body = _serializer.SerializeToBytes(integrationEvent);

            var publisher = InitPublisher(properties);

            publisher.Publish(properties: null, message: body);
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

        public void Subscribe<T>(SubscribeOptions properties)
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
                    subscriptionInfo.Channel.Close();
                    return;
                }
                else 
                {
                    _subscribers.Add(key, subscriptionInfo);
                }
            }

            _logger.LogInformation($"New subscription: {subscriptionInfo}");
        }

        public void Unsubscribe<T>(SubscribeOptions properties)
            where T : IIntegrationEvent
        {
            Utill.ThrowIfNull(properties, nameof(properties));
            Validate(properties.Exchange);

            var exchangeName = properties.Exchange.Name;
            var routingKey = properties.Queue?.RoutingKey ?? GetRoutingKey<T>();

            var key = GetSubscriptionKey(exchangeName, routingKey);

            Subscriber subscriptionInfo = null;
            
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

        private Subscriber CreateSubscriberChannel<T>(SubscribeOptions properties)
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

            return new Subscriber(eventType: typeof(T), channel, properties.Queue, properties.Exchange, properties.Consumer);
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            var key = GetSubscriptionKey(ea.Exchange, ea.RoutingKey);
            await ProcessMessage(ea, key);
        }

        private async Task ProcessMessage(BasicDeliverEventArgs ea, SubscriptionKey key)
        {
            Subscriber subscriptionInfo = null;
            var isSubscriberExist = false;

            using (new ReadLock(_syncRootForSubscribers)) 
                isSubscriberExist = _subscribers.TryGetValue(key, out subscriptionInfo);

            if (!isSubscriberExist) 
            {
                _logger.LogError($"Subscriber: {key} was not found.");
                return;
            }
            bool success = true;
            try
            {
                var integrationEvent = (IIntegrationEvent)_serializer.Deserialize(ea.Body.Span, subscriptionInfo.EventType);
                await _dispatcher.HandleAsync(integrationEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                success = ex is InvalidPermissionException || 
                          ex is InvalidDataException || 
                          ex is NotFoundException;
            }
            finally 
            {
                if (!subscriptionInfo.Consumer.Autoack) 
                {
                    if (success) 
                        subscriptionInfo.Channel.BasicAck(ea.DeliveryTag, false);
                    else
                        subscriptionInfo.Channel.BasicNack(ea.DeliveryTag, false, subscriptionInfo.Consumer.RequeueFailedMessages);
                }
            }
        }

        private SubscriptionKey GetSubscriptionKey(string exchange, string routingKey)
        {
            return new SubscriptionKey(exchange, routingKey);
        }

        private Publisher InitPublisher(PublishOptions properties)
        {
            if (!_connectionService.IsConnected)
                _connectionService.TryConnect();

            var exchange = properties.Exchange;
            var key = GetSubscriptionKey(exchange.Name, properties.RoutingKey);

            using (new ReadLock(_syncRootForPublishers))
            {
                if (_publishers.TryGetValue(key, out var publisher))
                    return publisher;
            }

            var newChannel = _connectionService.CreateChannel();
            newChannel.ExchangeDeclare(exchange: exchange.Name, type: exchange.Type, durable: exchange.Durable, autoDelete: exchange.AutoDelete);

            using (new WriteLock(_syncRootForPublishers))
            {
                if (_publishers.TryGetValue(key, out var existedPublisher))
                {
                    if (!newChannel.IsClosed)
                    {
                        newChannel.Close();
                    }

                    return existedPublisher;
                }
                else
                {
                    var publisher = new Publisher(exchange, properties.RoutingKey, newChannel);
                    _publishers.Add(key, publisher);
                    _logger.LogInformation($"Added publisher: {publisher}");
                    return publisher;
                }
            }
        }

        #endregion
    }
}
