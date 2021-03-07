using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.DataAccess;

namespace ProjectX.Realtime.Setup
{
    public static class RealtimeServiceCollectionExtensions
    {
        public static IServiceCollection AddRealtimeServices(this IServiceCollection services) 
            => services.AddScoped<IRealtimeTransactionContext, RealtimeTransactionContext>()
                       .AddTransient<INotificationHandler<TransactionCommitedEvent>, TransactionCommitedRealtimeHandler>();   
    }
}
