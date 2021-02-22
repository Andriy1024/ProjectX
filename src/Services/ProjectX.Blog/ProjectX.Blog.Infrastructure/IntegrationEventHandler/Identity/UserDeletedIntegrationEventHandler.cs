using MediatR;
using ProjectX.Blog.Application;
using ProjectX.Contracts.IntegrationEvents;
using ProjectX.Core.IntegrationEvents;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.IntegrationEventHandler.Identity
{
    public sealed class UserDeletedIntegrationEventHandler : IIntegrationEventHandler<UserDeletedIntegrationEvent>
    {
        private readonly IAuthorRepository _repository;

        public UserDeletedIntegrationEventHandler(IAuthorRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UserDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
        {
            var author = await _repository.FirstOrDefaultAsync(a => a.Id == integrationEvent.UserId, cancellationToken);

            if (author.IsFailed)
                return Unit.Value;

            _repository.Remove(author.Result);

            await _repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
