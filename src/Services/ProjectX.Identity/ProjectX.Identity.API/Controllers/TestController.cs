using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectX.Identity.API.Controllers
{
    [Route("api/test")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Test() 
        {
            return Ok("WORK");
        }
    }
}