using System;

namespace ProjectX.MessageBus
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        public string Exchange { get; }
        public string RoutingKey { get; }
        public string Queue { get; }

        /// <summary>
        /// Acknowlage
        /// </summary>
        public bool AutoDelete { get; }

        /// <summary>
        /// true - queue can has only one consumer
        /// </summary>
        public bool Exclusive { get; }

        /// <summary>
        /// Store message on disk
        /// </summary>
        public bool Durable { get; }
    }
}
