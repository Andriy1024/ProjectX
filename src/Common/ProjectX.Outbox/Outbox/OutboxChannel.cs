using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace ProjectX.Outbox
{
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
        /// We don't care too much if it succeeds because we have a fallback to handle "forgotten" messages.
        /// </summary>
        public void WriteNewMessages(Guid messageId) 
        {
            _messageIdChannel.Writer.TryWrite(messageId);            
        }

        public IAsyncEnumerable<Guid> ReadMessageIdsAsync(CancellationToken cancellationToken) 
        {
            return _messageIdChannel.Reader.ReadAllAsync(cancellationToken);
        }
    }
}
