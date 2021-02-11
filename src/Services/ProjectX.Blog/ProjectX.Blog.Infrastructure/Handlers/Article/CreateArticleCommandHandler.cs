using ProjectX.Blog.Application;
using ProjectX.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.Handlers
{
    public sealed class CreateArticleCommandHandler : ICommandHandler<CreateArticleCommand, ArticleDto>
    {
        public Task<IResponse<ArticleDto>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
