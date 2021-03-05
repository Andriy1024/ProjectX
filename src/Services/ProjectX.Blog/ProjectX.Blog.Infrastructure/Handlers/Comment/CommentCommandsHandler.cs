using AutoMapper;
using ProjectX.Blog.Application;
using ProjectX.Blog.Domain;
using ProjectX.Core;
using ProjectX.Core.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Blog.Infrastructure.Handlers
{
    public sealed class CommentCommandsHandler 
        : ICommandHandler<CreateCommentCommand, CommentDto>,
          ICommandHandler<UpdateCommentCommand, CommentDto>,
          ICommandHandler<DeleteCommentCommand>
    {
        private readonly IArticleRepository _articleRepository;
        private readonly ICurrentUser _currentUser;
        private readonly ICommentRepository _commentRepository;
        private readonly IMapper _mapper;

        public CommentCommandsHandler(IArticleRepository articleRepository, ICurrentUser currentUser, ICommentRepository commentRepository, IMapper mapper)
        {
            _articleRepository = articleRepository;
            _currentUser = currentUser;
            _commentRepository = commentRepository;
            _mapper = mapper;
        }

        public async Task<IResponse<CommentDto>> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
        {
            if (!await _articleRepository.ExistAsync(a => a.Id == command.ArticleId)) 
            {
                return ResponseFactory.NotFound<CommentDto>(ErrorCode.ArticleNotFound);
            }

            var comment = new CommentEntity(authorId: _currentUser.IdentityId,
                                            articleId: command.ArticleId,
                                            text: command.Text);

            await _commentRepository.InsertAsync(comment);
            await _commentRepository.UnitOfWork.SaveEntitiesAsync();
            var response = _mapper.Map<CommentDto>(comment);
            return ResponseFactory.Success(response);
        }

        public async Task<IResponse> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
        {
            var maybeComment = await _commentRepository.FirstOrDefaultAsync(c => c.Id == command.Id);
            
            if (maybeComment.IsFailed) 
            {
                return ResponseFactory.Failed(maybeComment.Error);
            }

            var comment = maybeComment.Result;
            comment.Delete(_currentUser.IdentityId);
            _commentRepository.Remove(comment);
            await _commentRepository.UnitOfWork.SaveEntitiesAsync();
            return ResponseFactory.Success();
        }

        public async Task<IResponse<CommentDto>> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
        {
            var maybeComment = await _commentRepository.FirstOrDefaultAsync(c => c.Id == command.Id);

            if (maybeComment.IsFailed)
            {
                return ResponseFactory.Failed<CommentDto>(maybeComment.Error);
            }

            var comment = maybeComment.Result;
            comment.Update(comment.Text, _currentUser.IdentityId);
            await _commentRepository.UnitOfWork.SaveEntitiesAsync();
            var response = _mapper.Map<CommentDto>(comment);
            return ResponseFactory.Success(response);
        }
    }
}
