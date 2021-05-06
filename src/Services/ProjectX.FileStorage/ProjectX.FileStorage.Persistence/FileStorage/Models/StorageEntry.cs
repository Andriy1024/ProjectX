using ProjectX.Core;
using ProjectX.FileStorage.Domain;

namespace ProjectX.FileStorage.Persistence.FileStorage.Models
{
    public class StorageEntry : IStorageEntry
    {
        public StorageEntry() {}

        public StorageEntry(string name, string location, string extension, string mimeType, long size)
        {
            Utill.ThrowIfNullOrEmpty(name, nameof(name));

            Name = name;
            Location = location;
            Extension = extension;
            MimeType = mimeType;
            Size = size;
        }

        public string Name { get; set;  }

        public string Location { get; set; }

        public string Extension { get; set; }

        public string MimeType { get; set; }

        public long Size { get; set; }
    }
}
