namespace ProjectX.RabbitMq
{
    public class ConsumerProperties 
    {
        /// <summary>
        /// Limit the number of unacknowledged messages on a channel (or connection) when consuming (aka "prefetch count").
        /// Channel.BasicQos(0, 10, false); Fetch 10 messages per consumer. But all messages handling in order.
        /// Вызов Channel.BasicQos(0, 1, false) означает, что ваш потребитель будет получать только одно готовое сообщение за раз от RabbitMQ, и пока не будет вызван BasicAck , другое сообщение не будет доставлено.
        /// Global flag should be flase, it applies PrefetchCount separately to each new consumer on the channel.
        /// Please note that if your client auto-acks messages, the prefetch value will have no effect.
        /// If you have one single or only a few consumers processing messages quickly, we recommend prefetching many messages at once to keep your client as busy as possible. 
        /// </summary>
        public ushort PrefetchCount { get; set; } = 10;

        /// <summary>
        ///  Channel.BasicAck(ea.DeliveryTag, false);
        ///  Channel.BasicConsume(queue: "queue.1", autoAck: false, consumer: consumer);
        ///  Please note that if your client auto-acks messages, the prefetch value will have no effect.
        /// </summary>
        public bool Autoack { get; set; } = true;

        public bool RequeueFailedMessages { get; set; } = false;

        public override string ToString()
        {
            return $"{nameof(PrefetchCount)}: {PrefetchCount}, {nameof(Autoack)}: {Autoack}.";
        }
    }
}
