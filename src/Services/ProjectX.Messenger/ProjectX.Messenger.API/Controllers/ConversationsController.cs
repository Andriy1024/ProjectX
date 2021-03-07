using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Infrastructure.Controllers;
using ProjectX.Messenger.Application;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Messenger.API.Controllers
{
    [Route("api/conversations")]
    [Authorize]
    public class ConversationsController : BaseApiController
    {
        [HttpPost]
        public Task<IActionResult> SendMessageAsync([FromBody] SendMessage command) => Send(command);

        [HttpPut]
        public Task<IActionResult> UpdateMessageAsync([FromBody] UpdateMessage command) => Send(command);

        [HttpDelete]
        public Task<IActionResult> DeleteMessageAsync([FromBody] DeleteMessage command) => Send(command);

        [HttpGet]
        public Task<IActionResult> GetConversationViewAsync([FromQuery] ConversationViewQuery query, CancellationToken cancellation) => Send(query, cancellation);

        [HttpGet("my")]
        public Task<IActionResult> GetConversationViewAsync(CancellationToken cancellation) => Send(new UserConversationsQuery(), cancellation);
    }
}