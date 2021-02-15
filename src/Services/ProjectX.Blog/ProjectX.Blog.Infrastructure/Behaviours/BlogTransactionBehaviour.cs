using Microsoft.Extensions.Logging;
using ProjectX.Blog.Persistence;
using ProjectX.Core;
using ProjectX.Core.IntegrationEvents;
using ProjectX.Infrastructure.CQRS;

namespace ProjectX.Blog.Infrastructure.Behaviours
{
    public sealed class BlogTransactionBehaviour<TRequest, TResponse> : TransactionBehaviour<TRequest, TResponse, BlogDbContext>
        where TRequest : IHasTransaction
    {
        public BlogTransactionBehaviour(ILogger<TransactionBehaviour<TRequest, TResponse, BlogDbContext>> logger,
            BlogDbContext dbContext, 
            IIntegrationEventService integrationEventService) 
            : base(logger, dbContext, integrationEventService)
        {
        }
    }
}
