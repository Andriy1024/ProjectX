using AutoMapper;
using ProjectX.Blog.Application;
using ProjectX.Blog.Domain;
using ProjectX.Core;
using ProjectX.Core.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.Handlers
{
    public sealed class CreateArticleCommandHandler : CommandHandler<CreateArticleCommand, ArticleDto>
    {
        private readonly IMapper _mapper;
        private readonly IArticleRepository _articleRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IAuthorRepository _authorRepository;

        public CreateArticleCommandHandler(IMapper mapper, 
            IArticleRepository articleRepository, 
            ICurrentUser currentUser, 
            IAuthorRepository authorRepository)
        {
            _mapper = mapper;
            _articleRepository = articleRepository;
            _currentUser = currentUser;
            _authorRepository = authorRepository;
        }

        public async override Task<IResponse<ArticleDto>> Handle(CreateArticleCommand command, CancellationToken cancellationToken)
        {
            var maybeAuthor = await _authorRepository.FirstOrDefaultAsync(a => a.Id == _currentUser.IdentityId);
            if (maybeAuthor.IsFailed)
                return ErrorResponse(maybeAuthor.Error);

            var article = ArticleEntity.Factory
                            .CreateArticle(command.Tittle, command.Body)
                            .Author(maybeAuthor.Result)
                            .Build();

            await _articleRepository.InsertAsync(article);
            await _articleRepository.UnitOfWork.SaveEntitiesAsync();
            
            return SuccessResponse(_mapper.Map<ArticleDto>(article));
        }
    }
}
