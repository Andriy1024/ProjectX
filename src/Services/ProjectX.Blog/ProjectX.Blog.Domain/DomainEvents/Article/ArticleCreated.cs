using ProjectX.Core;

namespace ProjectX.Blog.Domain
{
    public sealed class ArticleCreated : IDomainEvent
    {
        public ArticleCreated(ArticleEntity article)
        {
            Article = article;
        }

        public ArticleEntity Article { get; }
    }
}
