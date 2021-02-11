using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class UpdateArticleBodyCommand : ICommand<ArticleDto>
    {
        public long Id { get; set; }
        public string Body { get; set; }
    }
}
