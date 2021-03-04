using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Infrastructure.Controllers;
using ProjectX.Messenger.Application;
using System.Threading.Tasks;

namespace ProjectX.Messenger.API.Controllers
{
    [Route("api/conversations")]
    public class ConversationsController : BaseApiController
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendMessageAsync([FromBody] SendMessage command)
            => MapResponse(await Mediator.Send(command));

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateMessageAsync([FromBody] UpdateMessage command)
            => MapResponse(await Mediator.Send(command));

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteMessageAsync([FromBody] DeleteMessage command)
            => MapResponse(await Mediator.Send(command));

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetConversationViewAsync([FromQuery] GetConversationViewQuery query)
           => MapResponse(await Mediator.Send(query));
    }
}