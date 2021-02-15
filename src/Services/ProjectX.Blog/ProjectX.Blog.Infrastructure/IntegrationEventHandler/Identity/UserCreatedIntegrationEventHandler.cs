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

        public async Task Handle(UserCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
        {
            if (await _repository.ExistAsync(a => a.Id == integrationEvent.Id))
                return;

            var author = new AuthorEntity(id: integrationEvent.Id, 
                                          firstName: integrationEvent.FirstName,
                                          lastName: integrationEvent.LastName,
                                          email: integrationEvent.Email);

            await _repository.InsertAsync(author);
            await _repository.UnitOfWork.SaveEntitiesAsync();
        }
    }
}
