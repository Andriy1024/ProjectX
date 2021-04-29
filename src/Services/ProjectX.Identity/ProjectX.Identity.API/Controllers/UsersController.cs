using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Infrastructure.Controllers;
using ProjectX.Identity.Application;
using Microsoft.AspNetCore.Authorization;
using ProjectX.Core.Auth;
using ProjectX.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace ProjectX.Identity.API.Controllers
{
    [Route("api/users")]
    public class UsersController : BaseApiController
    {
        [HttpGet]
        //[Authorize]
        [ProducesResponseType(typeof(IResponse<UserDto[]>), 200)]
        public async Task<IActionResult> GetUsersAsync([FromQuery] UsersQuery query, CancellationToken cancellationToken)
            => MapResponse(await Mediator.Send(query, cancellationToken));

        [HttpGet("{id:long:min(1)}")]
        [Authorize]
        [ProducesResponseType(typeof(IResponse<UserDto>), 200)]
        public async Task<IActionResult> FindUserAsync([FromRoute] long id, CancellationToken cancellationToken)
            => MapResponse(await Mediator.Send(new FindUserQuery(id), cancellationToken));

        [HttpPost, Consumes("application/x-www-form-urlencoded")]
        [Authorize]
        [ProducesResponseType(typeof(IResponse<UserDto>), 200)]
        public async Task<IActionResult> CreateUserAsync([FromForm] CreateUserCommand command)
            => MapResponse(await Mediator.Send(command));

        [HttpPut("addresses")]
        [Authorize]
        [ProducesResponseType(typeof(IResponse<UserDto>), 200)]
        public async Task<IActionResult> UpdateAddressAsync([FromBody] UpdateAddressCommand command)
            => MapResponse(await Mediator.Send(command));

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpDelete("{id:long:min(1)}")]
        [ProducesResponseType(typeof(IResponse), 200)]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] long id)
            => MapResponse(await Mediator.Send(new DeleteUserCommand(id)));

        [HttpPost("send-verification-email")]
        [ProducesResponseType(typeof(IResponse), 200)]
        public async Task<IActionResult> SendEmailVerificationAsync([FromBody] SendEmailVerificationCommand command)
            => MapResponse(await Mediator.Send(command));

        [HttpPut("verify-email")]
        [ProducesResponseType(typeof(IResponse), 200)]
        public async Task<IActionResult> VerifyEmailAsync([FromBody] VerifyEmailCommand command)
            => MapResponse(await Mediator.Send(command));

        [HttpGet("redis")]
        public async Task<IActionResult> TestRedis([FromServices] IRedisCacheClient redis) 
        {
            return Ok(redis.Db0.Database.StringGet("mykey"));
        }
    }
}