using ProjectX.Core;
using System;

namespace ProjectX.FileStorage.Domain
{
    public sealed class FileEntity : Entity<Guid>, IStorageEntry
    {
        public string Name { get; }

        public string Location { get; }

        public string Extension { get; }

        public string MimeType { get; }

        public long Size { get; }

        public DateTime CreatedAt { get; }

        public FileEntity(Guid id, 
            string name, 
            string location, 
            string extension, 
            string mimeType, 
            long size,
            DateTime createdAt)
        {
            Id = id;
            Name = name;
            Location = location;
            Extension = extension;
            MimeType = mimeType;
            Size = size;
            CreatedAt = createdAt;
        }

        public static FileEntity Create(Guid id, DateTime createdAt, IStorageEntry storageEntry)
        {
            return new FileEntity(id: id, 
                                  name: storageEntry.Name, 
                                  extension: storageEntry.Extension, 
                                  location: storageEntry.Location,
                                  mimeType: storageEntry.MimeType,
                                  size: storageEntry.Size,
                                  createdAt: createdAt);
        }
    }
}
