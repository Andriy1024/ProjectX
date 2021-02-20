using MediatR;
using ProjectX.Blog.Application;
using ProjectX.Blog.Domain;
using ProjectX.Contracts.IntegrationEvents;
using ProjectX.Core.IntegrationEvents;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.IntegrationEventHandler.Identity
{
    public sealed class UserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
    {
        private readonly IAuthorRepository _repository;

        public UserCreatedIntegrationEventHandler(IAuthorRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UserCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
        {
            if (await _repository.ExistAsync(a => a.Id == integrationEvent.UserId))
                return Unit.Value;

            var author = new AuthorEntity(id: integrationEvent.UserId, 
                                          firstName: integrationEvent.FirstName,
                                          lastName: integrationEvent.LastName,
                                          email: integrationEvent.Email);

            await _repository.InsertAsync(author);
            await _repository.UnitOfWork.SaveEntitiesAsync();

            return Unit.Value;
        }
    }
}
