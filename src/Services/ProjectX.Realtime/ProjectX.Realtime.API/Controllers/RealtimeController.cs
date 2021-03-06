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
        private readonly WebSocketAuthenticationManager _authenticationManager;
        private readonly WebSocketConnectionManager _connectionManager;
        private readonly ICurrentUser _currentUser;

        public RealtimeController(WebSocketAuthenticationManager authenticationManager, 
                                  WebSocketConnectionManager connectionManager,
                                  ICurrentUser currentUser)
        {
            _authenticationManager = authenticationManager;
            _connectionManager = connectionManager;
            _currentUser = currentUser;
        }

        [Authorize]
        [HttpPost("connect")]
        public IActionResult GenerateConnectionId()
        {
            return Ok(new
            {
                ConnectionId = _authenticationManager.GenerateConnectionId(_currentUser).Value
            });
        }

        [Authorize(Roles = IdentityRoles.Admin)]
        [HttpGet("connections")]
        public IActionResult GetConnections()
        {
            var connections = _connectionManager.GetConnections();

            return Ok(connections.Select(c => new 
            {
                c.UserId,
                c.ConnectionId
            }));
        }
    }
}
