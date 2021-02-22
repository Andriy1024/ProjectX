using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Core.Auth;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    [ApiControllerAttribute]
    [Produces("application/json")]
    [Route("api")]
    [Authorize(Roles = IdentityRoles.Admin)]
    public class OutboxController : ControllerBase
    {
        private readonly OutboxDbContext _dbContext;

        public OutboxController(OutboxDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("outbox")]
        public async Task<IActionResult> GetOutboxMessagesAsync([FromQuery] int skip = 0, [FromQuery] int take = 100)
        {
            var messages = await _dbContext.OutboxMessages.Skip(skip).Take(take).ToArrayAsync();

            return Ok(messages.Select(m => new
            {
                m.Id,
                m.MessageType,
                m.SerializedMessage,
                m.SavedAt,
                m.SentAt
            }));
        }

        [HttpGet("inbox")]
        public async Task<IActionResult> GetInboxMessagesAsync([FromQuery] int skip = 0, [FromQuery] int take = 100)
        {
            var messages = await _dbContext.InboxMessages.Skip(skip).Take(take).ToArrayAsync();

            return Ok(messages.Select(m => new
            {
                m.Id,
                m.MessageType,
                m.ProcessedAt
            }));
        }
    }
}
