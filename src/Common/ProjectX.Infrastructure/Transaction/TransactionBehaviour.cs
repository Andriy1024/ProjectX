using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectX.Core;
using ProjectX.Core.DataAccess;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Infrastructure.Transaction
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IHasTransaction
    {
        protected readonly ILogger<TransactionBehaviour<TRequest, TResponse>> Logger;
        protected readonly IUnitOfWork TransactionManager;

        protected bool Success { get; private set; }

        protected TransactionBehaviour(ILogger<TransactionBehaviour<TRequest, TResponse>> logger, IUnitOfWork transaction)
        {
            Logger = logger;
            TransactionManager = transaction;
        }

        public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (TransactionManager.HasActiveTransaction) 
            {
                return await next();
            }
                
            var response = default(TResponse);
           
            await TransactionManager.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                using (var transaction = await TransactionManager.BeginTransactionAsync())
                {
                    Logger.LogInformation("----- Begin transaction {TransactionId} for ({@Command})", transaction.TransactionId, request);

                    response = await next();

                    Success = response is IResponse r ? r.IsSuccess : true; 

                    if (Success) 
                    {
                        await TransactionManager.CommitTransactionAsync(transaction);

                        Logger.LogInformation($"----- Transaction {transaction.TransactionId} was commited.");
                    }
                    else 
                    {
                        Logger.LogInformation($"----- Transaction {transaction.TransactionId} was failed.");
                    }
                }
            });

            return response;
        }
    }
}
