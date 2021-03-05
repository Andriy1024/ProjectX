using FluentValidation;
using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class UpdateCommentCommand : ICommand<CommentDto>
    {
        public long Id { get; set; }
        public string Text { get; set; }
    }

    public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
    {
        public UpdateCommentCommandValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0);
            RuleFor(c => c.Text).NotEmpty();
        }
    }
}
