using Microsoft.Extensions.Logging;
using ProjectX.Core;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Identity.Persistence;
using ProjectX.Infrastructure.CQRS;

namespace ProjectX.Identity.Infrastructure.Behaviours
{
    public sealed class IdentityTransactionBehaviour<TRequest, TResponse> : TransactionBehaviour<TRequest, TResponse, IdentityDbContext>
        where TRequest : IHasTransaction
    {
        public IdentityTransactionBehaviour(ILogger<TransactionBehaviour<TRequest, TResponse, IdentityDbContext>> logger, 
            IdentityDbContext dbContext, 
            IIntegrationEventService integrationEventService) 
            : base(logger, dbContext, integrationEventService)
        {
        }
    }
}
