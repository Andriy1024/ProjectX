using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class CreateArticleCommand : ICommand<ArticleDto>
    {
        public string Tittle { get; set; }
        public string Body { get; set; }
    }
}
