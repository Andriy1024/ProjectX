using FluentValidation;
using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class CreateCommentCommand : ICommand<CommentDto>
    {
        public long ArticleId { get; set; }

        public string Text { get; set; }
    }

    public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand> 
    {
        public CreateCommentCommandValidator()
        {
            RuleFor(c => c.ArticleId).GreaterThan(0);
            RuleFor(c => c.Text).NotEmpty();
        }
    }
}
