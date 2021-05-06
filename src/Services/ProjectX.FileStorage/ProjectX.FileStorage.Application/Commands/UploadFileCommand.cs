using Microsoft.AspNetCore.Http;
using ProjectX.Core;

namespace ProjectX.FileStorage.Application
{
    public sealed class UploadFileCommand : ICommand<FileDto>
    {
        public IFormFile File { get; set; }
        public string Location { get; set; }
    }
}
