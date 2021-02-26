using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Infrastructure.Controllers;
using ProjectX.Identity.Application;
using Microsoft.AspNetCore.Authorization;
using ProjectX.Core.Auth;

namespace ProjectX.Identity.API.Controllers
{
    [Route("api/users")]
    public class UsersController : BaseApiController
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsersAsync([FromQuery] UsersQuery query, CancellationToken cancellationToken)
            => MapResponse(await Mediator.Send(query, cancellationToken));

        [HttpGet("{id:long:min(1)}")]
        [Authorize]
        public async Task<IActionResult> FindUserAsync([FromRoute] long id, CancellationToken cancellationToken)
            => MapResponse(await Mediator.Send(new FindUserQuery(id), cancellationToken));

        [HttpPost, Consumes("application/x-www-form-urlencoded")]
        [Authorize]
        public async Task<IActionResult> CreateUserAsync([FromForm] CreateUserCommand command)
            => MapResponse(await Mediator.Send(command));

        [HttpPut("addresses")]
        [Authorize]
        public async Task<IActionResult> UpdateAddressAsync([FromBody] UpdateAddressCommand command)
            => MapResponse(await Mediator.Send(command));

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpDelete("{id:long:min(1)}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] long id)
            => MapResponse(await Mediator.Send(new DeleteUserCommand(id)));

        [HttpPost("send-verification-email")]
        public async Task<IActionResult> SendEmailVerificationAsync([FromBody] SendEmailVerificationCommand command)
            => MapResponse(await Mediator.Send(command));

        [HttpPut("verify-email")]
        public async Task<IActionResult> VerifyEmailAsync([FromBody] VerifyEmailCommand command)
            => MapResponse(await Mediator.Send(command));
    }
}