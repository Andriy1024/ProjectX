using Microsoft.EntityFrameworkCore;
using ProjectX.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox.AspNet
{
    public class GetOutboxMessagesQueryHandler : IQueryHandler<GetOutboxMessagesQuery, IEnumerable<OutboxMessageDto>>
    {
        private readonly OutboxDbContext _dbContext;

        public GetOutboxMessagesQueryHandler(OutboxDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IResponse<IEnumerable<OutboxMessageDto>>> Handle(GetOutboxMessagesQuery query, CancellationToken cancellationToken)
        {
            var messages = await _dbContext.OutboxMessages.ToArrayAsync(cancellationToken);

            return ResponseFactory.Success(messages.Select(m => new OutboxMessageDto(
                                                                    id: m.Id, 
                                                                    messageType: m.MessageType,
                                                                    serializedMessage: m.SerializedMessage, 
                                                                    savedAt: m.SavedAt,
                                                                    sentAt: m.SentAt)));
        }
    }
}
