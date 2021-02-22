using Microsoft.EntityFrameworkCore;
using ProjectX.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox.AspNet.Handlers
{
    public class GetInboxMessagesQueryHandler : IQueryHandler<GetInboxMessagesQuery, IEnumerable<InboxMessageDto>>
    {
        private readonly OutboxDbContext _dbContext;

        public GetInboxMessagesQueryHandler(OutboxDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IResponse<IEnumerable<InboxMessageDto>>> Handle(GetInboxMessagesQuery query, CancellationToken cancellationToken)
        {
            var messages = await _dbContext.InboxMessages.ToArrayAsync(cancellationToken);

            return ResponseFactory.Success(messages.Select(m => new InboxMessageDto(
                                                                    id: m.Id,
                                                                    messageType: m.MessageType,
                                                                    processedAt: m.ProcessedAt)));
        }
    }
}
