using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using ProjectX.Core;
using ProjectX.Core.Exceptions;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Core.Threading;
using ProjectX.MessageBus.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
        private readonly IMessageDispatcher _dispatcher;

        private readonly MessageBusConfiguration _options;
        private readonly CircuitBreakerPolicy _circuitBreaker;
     
        #endregion

        #region Constructors

        public RabbitMqMessageBus(IRabbitMqConnectionService connectionService,
            IMessageDispatcher dispatcher,
            IMessageSerializer serializer,
            ILogger<RabbitMqMessageBus> logger,
            IOptions<MessageBusConfiguration > options)
        {
            _connectionService = connectionService;
            _dispatcher = dispatcher;
            _serializer = serializer;
            _logger = logger;
            _options = MessageBusConfiguration.Validate(options.Value);
            _circuitBreaker = Policy.Handle<Exception>().CircuitBreaker(_options.Resilience.ExceptionsAllowedBeforeBreaking, TimeSpan.FromSeconds(_options.Resilience.DurationOfBreak));
        }

        #endregion

        #region IRabbitMqEventBus members

        public void AddPublisher(IEventBusProperties eventBusProperties)
        {
            var properties = PublishProperties.Validate(eventBusProperties);
            InitPublisher(properties);
        }

        public bool RemovePublisher(IEventBusProperties eventBusProperties)
        {
            var properties = PublishProperties.Validate(eventBusProperties);

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

                _logger.LogInformation($"Removed publisher: {publisher}");
            }

            return result;
        }

        public void Publish<T>(T integrationEvent, IEventBusProperties eventBusProperties)
            where T : IIntegrationEvent
        {
            var properties = PublishProperties.Validate(eventBusProperties, allowEmptyRoutingKey: true);

            if (properties.Exchange.IsFanout && string.IsNullOrEmpty(properties.RoutingKey))
                properties.RoutingKey = GetRoutingKey<T>();

            var body = _serializer.SerializeToBytes(integrationEvent);

            var publisher = InitPublisher(properties);

            var retryPolicy = Policy.Handle<SocketException>()
                                    .Or<BrokerUnreachableException>()
                                    .WaitAndRetry(_options.Resilience.RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                                    {
                                        _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                                    });

            _circuitBreaker.Wrap(retryPolicy).Execute(() => publisher.Publish(properties: null, message: body));
        }

        public void Subscribe<T>(IEventBusProperties eventBusProperties)
            where T : IIntegrationEvent
        {
            var properties = SubscribeProperties.Validate(eventBusProperties);

            if (string.IsNullOrEmpty(properties.Queue.RoutingKey))
                properties.Queue.RoutingKey = GetRoutingKey<T>();

            if (string.IsNullOrEmpty(properties.Queue.Name))
                properties.Queue.Name = GetQueueName(properties.Exchange.Name, properties.Queue.RoutingKey);

            var key = GetSubscriptionKey(properties.Exchange.Name, properties.Queue.RoutingKey);

            using (new ReadLock(_syncRootForSubscribers)) 
            {
                if (_subscribers.ContainsKey(key)) 
                {
                    _logger.LogError($"Integration event of type {typeof(T).Name} already subscribed for '{key}'.");
                    return;
                }
            }

            var subscriptionInfo = CreateSubscriberChannel<T>(key, properties);

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

        public void Unsubscribe<T>(IEventBusProperties eventBusProperties)
            where T : IIntegrationEvent
        {
            var properties = SubscribeProperties.Validate(eventBusProperties);

            var exchangeName = properties.Exchange.Name;
            var routingKey = properties.Queue?.RoutingKey ?? GetRoutingKey<T>();

            var key = GetSubscriptionKey(exchangeName, routingKey);

            Subscriber subscriptionInfo = null;
            
            bool exists = false;

            using (new ReadLock(_syncRootForSubscribers)) 
                exists = _subscribers.TryGetValue(key, out subscriptionInfo);
            
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
                _subscribers.Remove(key);
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

        private string GetRoutingKey<T>() => typeof(T).Name;

        private string GetQueueName(string exchange, string routingKey)
            => $"{_options.ConnectionName}/{exchange}.{routingKey}";

        private void Validate(ExchangeOptions exchange)
        {
            Utill.ThrowIfNull(exchange, nameof(exchange));
            Utill.ThrowIfNull(exchange.Name, nameof(exchange.Name));
            Utill.ThrowIfNull(exchange.Type, nameof(exchange.Type));
        }

        private Subscriber CreateSubscriberChannel<T>(SubscriptionKey key, SubscribeProperties properties)
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

                CreateSubscriberChannel<T>(key, properties);
            };

            return new Subscriber(key, eventType: typeof(T), channel, properties.Queue, properties.Exchange, properties.Consumer);
        }

        private async Task OnMessageReceived(object sender, BasicDeliverEventArgs ea)
        {
            var key = GetSubscriptionKey(ea.Exchange, ea.RoutingKey);

            Subscriber subscriptionInfo = null;
            var isSubscriberExist = false;

            using (new ReadLock(_syncRootForSubscribers))
                isSubscriberExist = _subscribers.TryGetValue(key, out subscriptionInfo);

            if (!isSubscriberExist)
            {
                _logger.LogError($"Subscriber: {key} was not found.");
                return;
            }

            var message = (IIntegrationEvent)_serializer.Deserialize(ea.Body.Span, subscriptionInfo.EventType);

            await ProcessMessage(message, subscriptionInfo, ea);
        }

        private async Task ProcessMessage(IIntegrationEvent message, Subscriber subscriptionInfo, BasicDeliverEventArgs ea)
        {
            var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(2, i => TimeSpan.FromSeconds(2));
            bool success = true;
            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    try
                    {
                        await _dispatcher.HandleAsync(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);

                        if (IsCustomError(ex))
                            return;

                        throw;
                    }
                });
            }
            catch
            {
                success = false;
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

        private bool IsCustomError(Exception ex) => ex switch 
        {
            InvalidPermissionException e => true,
            InvalidDataException e => true,
            NotFoundException e => true,
            _ => false
        };

        private SubscriptionKey GetSubscriptionKey(string exchange, string routingKey)
            => new SubscriptionKey(exchange, routingKey);
        
        private Publisher InitPublisher(PublishProperties properties)
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
