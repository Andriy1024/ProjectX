using ProjectX.Core;
using System.Collections.Generic;

namespace ProjectX.Outbox.AspNet
{
    public class GetInboxMessagesQuery : IQuery<IEnumerable<InboxMessageDto>>
    {
    }
}
