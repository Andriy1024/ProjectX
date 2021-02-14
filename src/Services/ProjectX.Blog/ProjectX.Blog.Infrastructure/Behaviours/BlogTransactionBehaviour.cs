using ProjectX.Blog.Persistence;
using ProjectX.Infrastructure.CQRS;
using System;

namespace ProjectX.Blog.Infrastructure.Behaviours
{
    public sealed class BlogTransactionBehaviour<TRequest, TResponse> : TransactionBehaviour<TRequest, TResponse, BlogDbContext>
    {
        public BlogTransactionBehaviour(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
