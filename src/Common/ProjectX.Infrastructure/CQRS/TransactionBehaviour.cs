using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectX.Core;
using ProjectX.Core.DataAccess;
using ProjectX.Core.IntegrationEvents;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Infrastructure.CQRS
{
    public abstract class TransactionBehaviour<TRequest, TResponse, TDbContext> : IPipelineBehavior<TRequest, TResponse>
        where TDbContext : DbContext, ITransactionActions
        where TRequest : IHasTransaction
    {
        protected readonly ILogger<TransactionBehaviour<TRequest, TResponse, TDbContext>> Logger;
        protected readonly TDbContext DbContext;
        protected readonly IIntegrationEventService IntegrationEventService;

        protected TransactionBehaviour(ILogger<TransactionBehaviour<TRequest, TResponse, TDbContext>> logger, TDbContext dbContext, IIntegrationEventService integrationEventService)
        {
            Logger = logger;
            DbContext = dbContext;
            IntegrationEventService = integrationEventService;
        }

        public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (DbContext.HasActiveTransaction)
                return await next();

            var response = default(TResponse);
            bool success = true;

            await DbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using (var transaction = await DbContext.BeginTransactionAsync())
                {
                    Logger.LogInformation("----- Begin transaction {TransactionId} for ({@Command})", transaction.TransactionId, request);

                    response = await next();

                    if (response is IResponse r)
                        success = r.IsSuccess;

                    Logger.LogInformation("----- Commit transaction {TransactionId}", transaction.TransactionId);

                    if (success)
                        await DbContext.CommitTransactionAsync(transaction);
                }

                if (success)
                    await IntegrationEventService.PublishEventsThroughEventBusAsync();
            });

            return response;
        }
    }
}
