using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectX.Core;
using ProjectX.Core.IntegrationEvents;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Infrastructure.Transaction
{
    public abstract class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IHasTransaction
    {
        protected readonly ILogger<TransactionBehaviour<TRequest, TResponse>> Logger;
        protected readonly ITransactionManager TransactionManager;
        protected readonly IIntegrationEventService IntegrationEventService;

        protected TransactionBehaviour(ILogger<TransactionBehaviour<TRequest, TResponse>> logger, 
            ITransactionManager transaction,
            IIntegrationEventService integrationEventService)
        {
            Logger = logger;
            TransactionManager = transaction;
            IntegrationEventService = integrationEventService;
        }

        public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (TransactionManager.HasActiveTransaction)
                return await next();

            var response = default(TResponse);
           
            await TransactionManager.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using (var transaction = await TransactionManager.BeginTransactionAsync())
                {
                    Logger.LogInformation("----- Begin transaction {TransactionId} for ({@Command})", transaction.TransactionId, request);

                    response = await next();

                    bool success = response is IResponse r ? r.IsSuccess : true; 

                    Logger.LogInformation("----- Commit transaction {TransactionId}", transaction.TransactionId);

                    if (success)
                        await TransactionManager.CommitTransactionAsync(transaction);
                }
            });

            return response;
        }
    }
}
