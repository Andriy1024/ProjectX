using ProjectX.FileStorage.Application;
using ProjectX.FileStorage.Domain;
using ProjectX.FileStorage.Persistence.Database.Documents;

namespace ProjectX.FileStorage.Persistence.Database.Extensions
{
    public static class MapperExtensions
    {
        public static FileDocument AsDocument(this FileEntity entity)
            => new FileDocument(id: entity.Id,
                                name: entity.Name,
                                location: entity.Location,
                                extension: entity.Extension,
                                mimeType: entity.MimeType,
                                size: entity.Size,
                                createdAt: entity.CreatedAt);

        public static FileEntity AsEntity(this FileDocument document)
            => new FileEntity(id: document.Id,
                              name: document.Name,
                              location: document.Location,
                              extension: document.Extension,
                              mimeType: document.MimeType,
                              size: document.Size,
                              createdAt: document.CreatedAt);

        public static FileDto AsDto(this FileDocument document)
            => new FileDto(id: document.Id,
                           name: document.Name,
                           location: document.Location,
                           extension: document.Extension,
                           mimeType: document.MimeType,
                           size: document.Size,
                           createdAt: document.CreatedAt);
    }
}
