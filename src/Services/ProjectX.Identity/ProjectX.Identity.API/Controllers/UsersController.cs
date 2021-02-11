using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Infrastructure.Controllers;
using ProjectX.Identity.Application;

namespace ProjectX.Identity.API.Controllers
{
    [Route("api/users")]
    public class UsersController : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync([FromQuery] UsersQuery query, CancellationToken cancellationToken) 
            => MapResponse(await Mediator.Send(query, cancellationToken));

        [HttpGet("{id:long:min(1)}")]
        public async Task<IActionResult> FindUserAsync([FromRoute] long id, CancellationToken cancellationToken)
            => MapResponse(await Mediator.Send(new FindUserQuery(id), cancellationToken));

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync([FromForm] CreateUserCommand command)
            => MapResponse(await Mediator.Send(command));

        [HttpPut("addresses")]
        public async Task<IActionResult> UpdateAddressAsync([FromBody] UpdateAddressCommand command)
            => MapResponse(await Mediator.Send(command));
    }
}