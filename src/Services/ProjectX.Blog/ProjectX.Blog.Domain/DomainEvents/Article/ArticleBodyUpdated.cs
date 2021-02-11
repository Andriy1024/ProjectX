using ProjectX.Core;

namespace ProjectX.Blog.Domain
{
    public sealed class ArticleBodyUpdated : IDomainEvent
    {
        public ArticleBodyUpdated(ArticleEntity article)
        {
            Article = article;
        }

        public ArticleEntity Article { get; }
    }
}
