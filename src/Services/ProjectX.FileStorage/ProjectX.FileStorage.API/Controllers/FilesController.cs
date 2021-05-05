using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectX.FileStorage.Application.Interfaces;
using ProjectX.FileStorage.Persistence.FileStorage.Local;
using ProjectX.FileStorage.Persistence.FileStorage.Models;
using ProjectX.Infrastructure.Controllers;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace ProjectX.FileStorage.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : BaseApiController
    {
        private readonly IFileStorage _storage;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _location = "storage";

        public FilesController(IFileStorage storage, IWebHostEnvironment webHostEnvironment)
        {
            _storage = storage;
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

        [HttpGet("directory-files")]
        public async Task<IActionResult> DirectoryFiles()
        {
            var files = Directory.GetFiles(Path.Combine(LocalFileStorage.RootLocation, _location));
            return Ok(files);
        }

        [HttpPost]
        public async Task<IActionResult> UploadAsync(IFormFile file, [FromQuery] string location) 
        {
            var uploadOptions = new UploadOptions(file, location);

            var fileInfo = await _storage.UploadAsync(uploadOptions);

            return Ok(fileInfo);
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadAsync([FromQuery] string name, [FromQuery] string location)
        {
            return File(await _storage.DownloadAsync(new DownloadOptions(Path.Combine(location, name))), "image/*");
        }
    }
}
