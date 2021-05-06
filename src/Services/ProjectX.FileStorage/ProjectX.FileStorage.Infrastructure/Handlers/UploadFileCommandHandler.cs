using AutoMapper;
using ProjectX.Core;
using ProjectX.FileStorage.Application;
using ProjectX.FileStorage.Application.Interfaces;
using ProjectX.FileStorage.Domain;
using ProjectX.FileStorage.Persistence.Database.Abstractions;
using ProjectX.FileStorage.Persistence.Database.Documents;
using ProjectX.FileStorage.Persistence.Database.Extensions;
using ProjectX.FileStorage.Persistence.FileStorage.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.FileStorage.Infrastructure.Handlers
{
    public sealed class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, FileDto>
    {
        private readonly IFileStorage _fileStorage;
        private readonly IMongoRepository<FileDocument, Guid> _repository;
        private readonly IMapper _mapper;

        public UploadFileCommandHandler(IFileStorage fileStorage, IMongoRepository<FileDocument, Guid> repository, IMapper mapper)
        {
            _fileStorage = fileStorage;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IResponse<FileDto>> Handle(UploadFileCommand command, CancellationToken cancellationToken)
        {
            var uploadOptions = new UploadOptions(command.File, command.Location);
           
            var storageEntry = await _fileStorage.UploadAsync(uploadOptions, cancellationToken);

            var entity = FileEntity.Create(Guid.NewGuid(), DateTime.UtcNow, storageEntry);

            await _repository.AddAsync(entity.AsDocument(), cancellationToken);

            return ResponseFactory.Success(_mapper.Map<FileDto>(entity));
        }
    }
}
