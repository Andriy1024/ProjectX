using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.DataAccess;
using System;

namespace ProjextX.DataAccess.Extensions
{
    public static class DataAccessServiceCollectionExtensions
    {
        public static IServiceCollection AddDbServices<T>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
            where T : DbContext
        {
            return services.AddDbContext<T>(optionsAction)
                           .AddScoped<IUnitOfWork, UnitOfWork<T>>();
        }

        public static IServiceCollection AddTransactinBehaviour(this IServiceCollection services)
             => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));
    }
}
