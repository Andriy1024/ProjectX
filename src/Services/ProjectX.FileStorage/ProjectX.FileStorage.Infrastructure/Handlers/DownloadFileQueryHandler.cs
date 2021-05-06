using ProjectX.Core;
using ProjectX.FileStorage.Application;
using ProjectX.FileStorage.Application.Interfaces;
using ProjectX.FileStorage.Persistence.Database.Abstractions;
using ProjectX.FileStorage.Persistence.Database.Documents;
using ProjectX.FileStorage.Persistence.FileStorage.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.FileStorage.Infrastructure.Handlers
{
    public sealed class DownloadFileQueryHandler : IQueryHandler<DownloadFileQuery, DownloadFileQuery.Response>
    {
        private readonly IFileStorage _fileStorage;
        private readonly IMongoRepository<FileDocument, Guid> _repository;

        public DownloadFileQueryHandler(IFileStorage fileStorage, IMongoRepository<FileDocument, Guid> repository)
        {
            _fileStorage = fileStorage;
            _repository = repository;
        }

        public async Task<IResponse<DownloadFileQuery.Response>> Handle(DownloadFileQuery query, CancellationToken cancellationToken)
        {
            var fileDocument = await _repository.GetAsync(query.Id, cancellationToken);

            if (fileDocument == null)
            {
                return ResponseFactory.NotFound<DownloadFileQuery.Response>(ErrorCode.NotFound);
            }

            var file = await _fileStorage.DownloadAsync(new DownloadOptions(fileDocument.Location, fileDocument.Name), cancellationToken);

            return ResponseFactory.Success(new DownloadFileQuery.Response(file, fileDocument.MimeType));
        }
    }
}
