using ProjectX.Blog.Application;
using ProjectX.Core;
using ProjectX.Core.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.Handlers
{
    public sealed class DeleteArticleCommandHandler : ICommandHandler<DeleteArticleCommand>
    {
        private readonly IArticleRepository _repository;
        private readonly ICurrentUser _currentUser;

        public DeleteArticleCommandHandler(IArticleRepository repository, ICurrentUser currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<IResponse> Handle(DeleteArticleCommand command, CancellationToken cancellationToken)
        {
            var maybeArticle = await _repository.GetFullArticleAsync(a => a.Id == command.Id);
            if (maybeArticle.IsFailed)
                return ResponseFactory.Failed(maybeArticle.Error);

            var article = maybeArticle.Result;

            if (article.AuthorId != _currentUser.IdentityId)
                return ResponseFactory.InvalidPermission(ErrorCode.InvalidPermission, "Article can be delete by owner only.");

            article.Delete();
            _repository.Remove(article);
            await _repository.UnitOfWork.SaveEntitiesAsync();

            return Response.Success;
        }
    }
}
