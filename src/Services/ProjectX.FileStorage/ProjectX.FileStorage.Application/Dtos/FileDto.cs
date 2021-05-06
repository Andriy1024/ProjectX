using System;

namespace ProjectX.FileStorage.Application
{
    public class FileDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string Extension { get; set; }

        public string MimeType { get; set; }

        public long Size { get; set; }

        public DateTime CreatedAt { get; set; }

        public FileDto()
        {

        }

        public FileDto(Guid id, 
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
