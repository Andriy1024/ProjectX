using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace ProjectX.Outbox
{
    /// <summary>
    /// The channel serves as bridge between <see cref="OutboxTransactionContext"/> and <see cref="OutboxChannelPublisher"/> channel publisher to reduce publish delay.
    /// </summary>
    public sealed class OutboxChannel
    {
        private readonly Channel<Guid> _messageIdChannel;

        public OutboxChannel()
        {
            _messageIdChannel = Channel.CreateUnbounded<Guid>(new UnboundedChannelOptions() 
            {
                SingleReader = true,
                SingleWriter = false
            });
        }

        /// <summary>
        /// We don't care too much if it succeeds because we have the OutboxFallbackPublisher to handle "forgotten" messages.
        /// </summary>
        public void WriteNewMessages(Guid messageId) 
        {
            _messageIdChannel.Writer.TryWrite(messageId);            
        }

        public IAsyncEnumerable<Guid> ReadMessagesIdsAsync(CancellationToken cancellationToken) 
        {
            return _messageIdChannel.Reader.ReadAllAsync(cancellationToken);
        }
    }
}
