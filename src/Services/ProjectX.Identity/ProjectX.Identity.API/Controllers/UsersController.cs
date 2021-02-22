using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Infrastructure.Controllers;
using ProjectX.Identity.Application;
using ProjectX.Core.DataAccess;
using System;
using ProjectX.Contracts.IntegrationEvents;
using Microsoft.AspNetCore.Authorization;
using ProjectX.Core.Auth;

namespace ProjectX.Identity.API.Controllers
{
    [Route("api/users")]
    public class UsersController : SharedController
    {
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsersAsync([FromQuery] UsersQuery query, CancellationToken cancellationToken)
            => MapResponse(await Mediator.Send(query, cancellationToken));

        [HttpGet("{id:long:min(1)}")]
        [Authorize]
        public async Task<IActionResult> FindUserAsync([FromRoute] long id, CancellationToken cancellationToken)
            => MapResponse(await Mediator.Send(new FindUserQuery(id), cancellationToken));

        [HttpPost]
        [Authorize (Roles = IdentityRoles.Admin)]
        public async Task<IActionResult> CreateUserAsync([FromForm] CreateUserCommand command)
            => MapResponse(await Mediator.Send(command));

        [HttpPut("addresses")]
        [Authorize]
        public async Task<IActionResult> UpdateAddressAsync([FromBody] UpdateAddressCommand command)
            => MapResponse(await Mediator.Send(command));

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpDelete("{id:long:min(1)}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] long id, CancellationToken cancellationToken)
            => MapResponse(await Mediator.Send(new DeleteUserCommand(id), cancellationToken));

        [HttpGet("test")]
        public async Task<IActionResult> Test([FromServices] IUnitOfWork unit, CancellationToken cancellationToken)
        {
            var aa = typeof(UserDeletedIntegrationEvent).AssemblyQualifiedName;

            Type  a = Type.GetType(aa);
            
            return Ok(a?.Name);
        }
    }
}