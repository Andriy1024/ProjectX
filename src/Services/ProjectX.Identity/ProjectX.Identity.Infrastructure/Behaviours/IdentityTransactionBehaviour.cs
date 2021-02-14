using ProjectX.Identity.Persistence;
using ProjectX.Infrastructure.CQRS;
using System;

namespace ProjectX.Identity.Infrastructure.Behaviours
{
    public sealed class IdentityTransactionBehaviour<TRequest, TResponse> : TransactionBehaviour<TRequest, TResponse, IdentityDbContext>
    {
        public IdentityTransactionBehaviour(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
