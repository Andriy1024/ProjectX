using MongoDB.Bson.Serialization.Attributes;
using ProjectX.FileStorage.Domain;
using ProjectX.FileStorage.Persistence.Database.Abstractions;
using System;

namespace ProjectX.FileStorage.Persistence.Database.Documents
{
    public sealed class FileDocument : IStorageEntry, IIdentifiable<Guid>
    {
        [BsonId]
        public Guid Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        public string Location { get; set; }

        public string Extension { get; set; }

        public string MimeType { get; set; }

        public long Size { get; set; }

        public DateTime CreatedAt { get; set; }

        public FileDocument()
        {
        }

        public FileDocument(Guid id, 
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
    }
}
