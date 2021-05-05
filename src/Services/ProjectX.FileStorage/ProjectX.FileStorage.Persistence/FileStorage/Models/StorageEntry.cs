using ProjectX.Core;
using ProjectX.FileStorage.Persistence.FileStorage.Abstractions;

namespace ProjectX.FileStorage.Persistence.FileStorage.Models
{
    public class StorageEntry : IStorageEntry
    {
        public StorageEntry(string name, string location, string extension, string mimeType, long size)
        {
            Utill.ThrowIfNullOrEmpty(name, nameof(name));

            Name = name;
            Location = location;
            Extension = extension;
            MimeType = mimeType;
            Size = size;
        }

        public string Name { get; }

        public string Location { get; }

        public string Extension { get; }

        public string MimeType { get; }

        public long Size { get; }
    }
}
