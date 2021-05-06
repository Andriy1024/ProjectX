using Microsoft.AspNetCore.Http;
using ProjectX.Core;
using ProjectX.FileStorage.Application.SeedWork;
using ProjectX.FileStorage.Domain;
using System.IO;

namespace ProjectX.FileStorage.Persistence.FileStorage.Models
{
    public class UploadOptions
    {
        public UploadOptions(Stream file, IStorageEntry entry, bool @override = false)
        {
            Utill.ThrowIfNull(file, nameof(file));
            Utill.ThrowIfNull(entry, nameof(entry));

            EntryStream = file;
            EntryInfo = entry;
            Override = @override;
        }

        public UploadOptions(IFormFile file, string location, bool @override = false)
        {
            Utill.ThrowIfNull(file, nameof(file));
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
