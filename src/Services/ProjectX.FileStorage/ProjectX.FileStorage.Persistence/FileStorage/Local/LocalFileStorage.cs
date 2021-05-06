using ProjectX.Core;
using ProjectX.FileStorage.Application.Interfaces;
using ProjectX.FileStorage.Domain;
using ProjectX.FileStorage.Persistence.FileStorage.Models;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.FileStorage.Persistence.FileStorage.Local
{
    public sealed class LocalFileStorage : IFileStorage
    {
        public static string RootLocation = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "file_storage");

        public async Task<IStorageEntry> UploadAsync(UploadOptions options, CancellationToken cancellationToken = default)
        {
            Utill.ThrowIfNull(options, nameof(options));

            var physicalLocation = GetPhysicalPath(options.EntryInfo.Location);

            var physicalLocationWithName = Path.Combine(physicalLocation, options.EntryInfo.Name);

            if (!options.Override) 
            {
                ThrowIfFileExists(physicalLocationWithName);
            }

            if (Directory.Exists(physicalLocationWithName))
            {
                throw new Exception($"Cannot create file '{physicalLocationWithName}' because it already exists as a directory.");
            }

            CreateFolderIfNotExists(physicalLocation);

            var source = options.EntryStream;

            source.Seek(0, SeekOrigin.Begin);

            await using (var fileStream = new FileStream(physicalLocationWithName, FileMode.Create, FileAccess.Write))
            {
                await source.CopyToAsync(fileStream, cancellationToken);
            }

            return options.EntryInfo;
        }

        public async Task<Stream> DownloadAsync(DownloadOptions options, CancellationToken cancellationToken = default)
        {
            Utill.ThrowIfNull(options, nameof(options));

            var physicalLocation = GetPhysicalPath(options.Location);

            if (!File.Exists(physicalLocation))
            {
                throw new Exception("File not found.");
            }

            return new FileStream(physicalLocation, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public Task DeleteAsync(DeleteOptions options, CancellationToken cancellationToken = default)
        {
            Utill.ThrowIfNull(options, nameof(options));

            var physicalLocation = GetPhysicalPath(options.Location);

            if (!File.Exists(physicalLocation))
            {
                throw new Exception("File not found.");
            }

            File.Delete(physicalLocation);

            return Task.CompletedTask;
        }

        private string GetPhysicalPath(string path)
        {
            return Path.Combine(RootLocation, path);
        }

        private void ThrowIfFileExists(string path)
        {
            if (File.Exists(path))
            {
                throw new Exception($"Cannot create file '{path}' because it already exists.");
            }
        }

        private void CreateFolderIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
