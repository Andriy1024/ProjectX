using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Core;
using ProjectX.Core.DataAccess;
using ProjectX.Core.IntegrationEvents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Infrastructure.CQRS
{
    public abstract class TransactionBehaviour<TRequest, TResponse, TDbContext> : IPipelineBehavior<TRequest, TResponse>
        where TDbContext : DbContext, ITransactionActions
    {
        protected ILogger<TransactionBehaviour<TRequest, TResponse, TDbContext>> Logger { get; private set; }
        protected TDbContext DbContext { get; private set; }
        protected IIntegrationEventService IntegrationEventService { get; private set; }
        
        protected readonly IServiceProvider ServiceProvider;

        public TransactionBehaviour(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public virtual async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!(request is ICommand<TResponse> || request is ICommand))
                return await next();
            
            DbContext = ServiceProvider.GetRequiredService<TDbContext>();

            if (DbContext.HasActiveTransaction)
                return await next();

            Logger = ServiceProvider.GetRequiredService<ILogger<TransactionBehaviour<TRequest, TResponse, TDbContext>>>();
            IntegrationEventService = ServiceProvider.GetRequiredService<IIntegrationEventService>();

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
