using ProjectX.Core;
using System.Collections.Generic;

namespace ProjectX.Outbox.AspNet
{
    public class GetOutboxMessagesQuery : IQuery<IEnumerable<OutboxMessageDto>>
    {
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 100;
    }
}
