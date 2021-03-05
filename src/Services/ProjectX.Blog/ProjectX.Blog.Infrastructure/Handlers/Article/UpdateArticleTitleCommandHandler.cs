using AutoMapper;
using ProjectX.Blog.Application;
using ProjectX.Core;
using ProjectX.Core.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.Handlers
{
    public sealed class UpdateArticleTitleCommandHandler : ICommandHandler<UpdateArticleTitleCommand, ArticleDto>
    {
        private readonly IArticleRepository _repository;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public UpdateArticleTitleCommandHandler(IArticleRepository repository, ICurrentUser currentUser, IMapper mapper)
        {
            _repository = repository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<IResponse<ArticleDto>> Handle(UpdateArticleTitleCommand command, CancellationToken cancellationToken)
        {
            var maybeArticle = await _repository.GetFullArticleAsync(a => a.Id == command.Id);
            if (maybeArticle.IsFailed)
                return ResponseFactory.Failed<ArticleDto>(maybeArticle.Error);

            var article = maybeArticle.Result;

            if (article.AuthorId != _currentUser.IdentityId)
                return ResponseFactory.InvalidPermission<ArticleDto>(ErrorCode.InvalidPermission, "Article can be updated by owner only.");

            article.UpdateTitle(command.Title);

            await _repository.UnitOfWork.SaveEntitiesAsync();

            return ResponseFactory.Success(_mapper.Map<ArticleDto>(article));
        }
    }
}
