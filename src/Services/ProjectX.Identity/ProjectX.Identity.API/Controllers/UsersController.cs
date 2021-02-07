using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjectX.Common.Infrastructure.Controllers;

namespace ProjectX.Identity.API.Controllers
{
    [Route("api/users")]
    public class UsersController : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync() =>
            Ok();
    }
}