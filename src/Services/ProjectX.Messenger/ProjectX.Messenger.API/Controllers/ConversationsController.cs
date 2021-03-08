using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Core;
using ProjectX.Infrastructure.Controllers;
using ProjectX.Messenger.Application;
using ProjectX.Messenger.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Messenger.API.Controllers
{
    [Route("api/conversations")]
    [Authorize]
    public class ConversationsController : BaseApiController
    {
        [HttpPost]
        [ProducesResponseType(typeof(IResponse), 200)]
        public Task<IActionResult> SendMessageAsync([FromBody] SendMessage command) => Send(command);

        [HttpPut]
        [ProducesResponseType(typeof(IResponse), 200)]
        public Task<IActionResult> UpdateMessageAsync([FromBody] UpdateMessage command) => Send(command);

        [HttpDelete]
        [ProducesResponseType(typeof(IResponse), 200)]
        public Task<IActionResult> DeleteMessageAsync([FromBody] DeleteMessage command) => Send(command);

        [HttpGet]
        [ProducesResponseType(typeof(IResponse<ConversationView>), 200)]
        public Task<IActionResult> GetConversationViewAsync([FromQuery] ConversationViewQuery query, CancellationToken cancellation) => Send(query, cancellation);

        [HttpGet("my")]
        [ProducesResponseType(typeof(IResponse<List<UserConversationsView.Conversation>>), 200)]
        public Task<IActionResult> GetConversationViewAsync(CancellationToken cancellation) => Send(new UserConversationsQuery(), cancellation);
    }
}