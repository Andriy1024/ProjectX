using FluentValidation;
using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class UpdateArticleTitleCommand : ICommand<ArticleDto>
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }

    public class UpdateArticleTitleCommandValidator : AbstractValidator<UpdateArticleTitleCommand>
    {
        public UpdateArticleTitleCommandValidator()
        {
            RuleFor(c => c.Title).NotEmpty();
            RuleFor(c => c.Id).GreaterThan(0);
        }
    }
}
