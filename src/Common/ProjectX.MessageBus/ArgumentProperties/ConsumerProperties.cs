namespace ProjectX.RabbitMq
{
    public class ConsumerProperties 
    {
        /// <summary>
        /// Limit the number of unacknowledged messages on a channel (or connection) when consuming (aka "prefetch count").
        /// Channel.BasicQos(0, 10, false); Fetch 10 messages per consumer. But all messages handling in order.
        /// Виклик Channel.BasicQos(0, 1, false) означає, що підписник буде отримувати тільки одне готове повідомлення за раз від RabbitMQ, і доки не буде викликаний BasicAck , наступне повідомлення не буде доставлено.
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

        public bool RetryOnFailure { get; set; } = true;

        public override string ToString()
        {
            return $"{nameof(PrefetchCount)}: {PrefetchCount}, {nameof(Autoack)}: {Autoack}.";
        }
    }
}
