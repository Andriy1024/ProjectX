using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectX.Common.DataAccess;
using ProjectX.Common.Infrastructure.DataAccess;
using ProjectX.Common.IntegrationEvents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Common.Infrastructure.CQRS
{
    public abstract class TransactionBehaviour<TRequest, TResponse, TDbContext> : IPipelineBehavior<TRequest, TResponse>
        where TDbContext : DbContext, ITransactionActions
    {
        protected readonly ILogger<TransactionBehaviour<TRequest, TResponse, TDbContext>> Logger;
        protected readonly TDbContext DbContext;
        protected readonly IIntegrationEventService IntegrationEventService;

        public TransactionBehaviour(TDbContext dbContext,
            IIntegrationEventService integrationEventService,
            ILogger<TransactionBehaviour<TRequest, TResponse, TDbContext>> logger)
        {
            DbContext = dbContext ?? throw new ArgumentException(nameof(BaseDbContext<TDbContext>));
            IntegrationEventService = integrationEventService ?? throw new ArgumentException(nameof(integrationEventService));
            Logger = logger ?? throw new ArgumentException(nameof(ILogger));
        }

        public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!(request is ICommand<TResponse> || request is ICommand))
                return await next();

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
