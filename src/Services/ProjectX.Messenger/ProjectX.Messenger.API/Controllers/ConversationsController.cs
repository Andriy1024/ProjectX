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
        public async Task<IActionResult> SendMessageAsync([FromBody] SendMessage command)
            => MapResponse(await Mediator.Send(command));

        [HttpPut]
        public async Task<IActionResult> UpdateMessageAsync([FromBody] UpdateMessage command)
            => MapResponse(await Mediator.Send(command));

        [HttpDelete]
        public async Task<IActionResult> DeleteMessageAsync([FromBody] DeleteMessage command)
            => MapResponse(await Mediator.Send(command));

        [HttpGet]
        public async Task<IActionResult> GetConversationViewAsync([FromQuery] ConversationViewQuery query, CancellationToken cancellation)
           => MapResponse(await Mediator.Send(query, cancellation));

        [HttpGet("my")]
        public async Task<IActionResult> GetConversationViewAsync(CancellationToken cancellation)
           => MapResponse(await Mediator.Send(new UserConversationsQuery(), cancellation));
    }
}