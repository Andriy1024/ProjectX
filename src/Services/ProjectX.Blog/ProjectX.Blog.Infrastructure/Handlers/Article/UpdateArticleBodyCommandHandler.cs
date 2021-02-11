using AutoMapper;
using ProjectX.Blog.Application;
using ProjectX.Core;
using ProjectX.Core.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.Handlers
{
    public sealed class UpdateArticleBodyCommandHandler : ICommandHandler<UpdateArticleBodyCommand, ArticleDto>
    {
        readonly IArticleRepository _repository;
        readonly ICurrentUser _currentUser;
        readonly IMapper _mapper;

        public UpdateArticleBodyCommandHandler(IArticleRepository repository, ICurrentUser currentUser, IMapper mapper)
        {
            _repository = repository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<IResponse<ArticleDto>> Handle(UpdateArticleBodyCommand command, CancellationToken cancellationToken)
        {
            var maybeArticle = await _repository.GetFullArticleAsync(a => a.Id == command.Id);
            if (maybeArticle.IsFailed)
                return ResponseFactory.Failed<ArticleDto>(maybeArticle.Error);

            var article = maybeArticle.Result;

            if (article.AuthorId != _currentUser.IdentityId)
                return ResponseFactory.InvalidPermission<ArticleDto>(ErrorCode.InvalidPermission, "Article can be updated by owner only.");

            article.UpdateBody(command.Body);

            await _repository.UnitOfWork.SaveEntitiesAsync();

            return ResponseFactory.Success(_mapper.Map<ArticleDto>(article));
        }
    }
}
