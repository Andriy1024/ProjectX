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
        public IActionResult GetConnections([FromServices] WebSocketManager connectionManager)
        {
            return Ok(connectionManager.GetConnections().Select(c => new 
            {
                c.UserId,
                ConnectionId = c.ConnectionId.Value,
                State = c.GetWebSocketState()
            }));
        }

        [HttpGet("reverse-proxy-headers")]
        public IActionResult GetProxyHeadersAsync() 
        {
            HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var xfor);
            HttpContext.Request.Headers.TryGetValue("X-Forwarded-Proto", out var xproto);
            HttpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var xhost);
            HttpContext.Request.Headers.TryGetValue("X-Forwarded-Location", out var xlocation);

            return Ok(new
            {
                X_Forwarded_For = xfor.FirstOrDefault(),
                X_Forwarded_Proto = xproto.FirstOrDefault(),
                X_Forwarded_Host = xhost.FirstOrDefault(),
                X_Forwarded_Location = xlocation.FirstOrDefault()
            });
        }
    }
}
