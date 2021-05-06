using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectX.FileStorage.Application;
using ProjectX.Infrastructure.Controllers;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.FileStorage.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : BaseApiController
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FilesController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("host")]
        public async Task<IActionResult> GetHost()
        {
            return Ok(new {
                HttpHost = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}",
                AssemblyRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                WWWROOT = _webHostEnvironment.WebRootPath
            });
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync([FromForm] UploadFileCommand command, CancellationToken cancellation = default) 
        {
            return MapResponse(await Mediator.Send(command, cancellation));
        }

        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadAsync([FromRoute] Guid id, CancellationToken cancellation = default)
        {
            var response = await Mediator.Send(new DownloadFileQuery(id), cancellation);

            return File(response.Data.File, response.Data.MimeType);
        }

        [HttpGet]
        public async Task<IActionResult> GetFilesAsync(CancellationToken cancellationToken)
        {
            return MapResponse(await Mediator.Send(new FilesQuery(), cancellationToken));
        }
    }
}
