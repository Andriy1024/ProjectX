using RabbitMQ.Client;
using System.Threading.Tasks;
using static ProjectX.RabbitMq.Pipeline.Pipe;

namespace ProjectX.RabbitMq.Pipeline
{
    /// <summary>
    /// Delivery processing acknowledgements from consumers to RabbitMQ are known as acknowledgements in messaging protocols. 
    /// For more <a href="https://www.rabbitmq.com/confirms.html"/>.
    /// </summary>
    internal sealed class AcknowledgmentPipe : IPipeLine<SubscriberRequest>
    {
        private readonly IModel _channel;
        private readonly bool _requeueFailedMessages;

        public AcknowledgmentPipe(IModel channel, bool requeueFailedMessages)
        {
            _channel = channel;
            _requeueFailedMessages = requeueFailedMessages;
        }

        public async Task Handle(SubscriberRequest request, Handler<SubscriberRequest> next)
        {
            bool success = true;

            try
            {
                await next(request);
            }
            catch
            {
                success = false;
            }
            finally
            {
                if (success)
                {
                    _channel.BasicAck(request.RabbitPrperties.DeliveryTag, false);
                }
                else
                {
                    // in a REAL WORLD app this should be handled with a Dead Letter Exchange (DLX). 
                    // For more information see: https://www.rabbitmq.com/dlx.html
                    _channel.BasicNack(request.RabbitPrperties.DeliveryTag, false, _requeueFailedMessages);
                }
            }
        }
    }
}
