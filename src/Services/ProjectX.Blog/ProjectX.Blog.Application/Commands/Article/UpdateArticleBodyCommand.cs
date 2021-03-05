using FluentValidation;
using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class UpdateArticleBodyCommand : ICommand<ArticleDto>
    {
        public long Id { get; set; }
        public string Body { get; set; }
    }

    public class UpdateArticleBodyCommandValidator : AbstractValidator<UpdateArticleBodyCommand>
    {
        public UpdateArticleBodyCommandValidator()
        {
            RuleFor(c => c.Body).NotEmpty();
            RuleFor(c => c.Id).GreaterThan(0);
        }
    }
}
