using FluentValidation;
using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class CreateArticleCommand : ICommand<ArticleDto>
    {
        public string Tittle { get; set; }
        public string Body { get; set; }
    }

    public class CreateArticleCommandValidator : AbstractValidator<CreateArticleCommand>
    {
        public CreateArticleCommandValidator()
        {
            RuleFor(c => c.Body).NotEmpty();
            RuleFor(c => c.Tittle).NotEmpty();
        }
    }
}
