using Microsoft.AspNetCore.Http;
using ProjectX.FileStorage.Application.SeedWork;
using ProjectX.FileStorage.Persistence.FileStorage.Abstractions;
using System.IO;

namespace ProjectX.FileStorage.Persistence.FileStorage.Models
{
    public class DownloadOptions
    {
        public string Location { get; }

        public DownloadOptions(string location)
        {
            Location = location;
        }
    }

    public class DeleteOptions
    {
        public string Location { get; }

        public DeleteOptions(string location)
        {
            Location = location;
        }

    }

    public class UploadOptions
    {
        public UploadOptions(Stream file, IStorageEntry entry, bool @override = false)
        {
            EntryStream = file;
            EntryInfo = entry;
            Override = @override;
        }

        public UploadOptions(IFormFile file, string location, bool @override = false)
        {
            var size = file.Length;
            var extension = FileUtill.TryGetExtension(file.FileName);
            var mimeType = FileUtill.TryGetContentType(file.FileName);
            var name = FileUtill.GenerateFileName(extension);

            EntryStream = file.OpenReadStream();
            EntryInfo = new StorageEntry(name: name, location: location, extension: extension, mimeType: mimeType, size: size);
            Override = @override;
        }

        public Stream EntryStream { get; }

        public IStorageEntry EntryInfo { get; }

        /// <summary>
        /// Override file if it exists when true, either throw exception.
        /// </summary>
        public bool Override { get; }
    }
}
