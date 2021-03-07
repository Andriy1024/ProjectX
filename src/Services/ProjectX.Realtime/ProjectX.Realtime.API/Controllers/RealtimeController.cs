using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Core.Auth;
using ProjectX.Realtime.Infrastructure;
using System.Linq;

namespace ProjectX.Realtime.API.Controllers
{
    [ApiController]
    [Route("api/realtime")]
    public class RealtimeController : ControllerBase
    {
        [Authorize]
        [HttpPost("connect")]
        public IActionResult GenerateConnectionId([FromServices] WebSocketAuthenticationManager authenticationManager, [FromServices] ICurrentUser currentUser)
        {
            return Ok(new
            {
                ConnectionId = authenticationManager.GenerateConnectionId(currentUser).Value
            });
        }

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpGet("connections")]
        public IActionResult GetConnections([FromServices] WebSocketConnectionManager connectionManager)
        {
            return Ok(connectionManager.GetConnections().Select(c => new 
            {
                c.UserId,
                ConnectionId = c.ConnectionId.Value
            }));
        }
    }
}
