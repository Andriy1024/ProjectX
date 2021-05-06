using MongoDB.Driver;
using ProjectX.Core;
using ProjectX.FileStorage.Application;
using ProjectX.FileStorage.Persistence.Database.Abstractions;
using ProjectX.FileStorage.Persistence.Database.Documents;
using ProjectX.FileStorage.Persistence.Database.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.FileStorage.Infrastructure.Handlers
{
    public sealed class FilesQueryHandler : IQueryHandler<FilesQuery, FileDto[]>
    {
        private readonly IMongoRepository<FileDocument, Guid> _repository;

        public FilesQueryHandler(IMongoRepository<FileDocument, Guid> repository)
        {
            _repository = repository;
        }

        public async Task<IResponse<FileDto[]>> Handle(FilesQuery query, CancellationToken cancellationToken)
        {
            var files = await _repository.Collection.AsQueryable().ToListAsync(cancellationToken);

            return ResponseFactory.Success(files.Select(s => s.AsDto()).ToArray());
        }
    }
}
