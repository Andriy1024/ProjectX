using ProjectX.Core;

namespace ProjectX.Blog.Domain
{
    public sealed class ArticleTittleUpdated : IDomainEvent
    {
        public ArticleTittleUpdated(ArticleEntity article)
        {
            Article = article;
        }

        public ArticleEntity Article { get; }
    }
}
