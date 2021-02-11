using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class UpdateArticleTitleCommand : ICommand<ArticleDto>
    {
        public long Id { get; set; }
        public string Title { get; set; }
    }
}
