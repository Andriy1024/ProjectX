using ProjectX.Core;

namespace ProjectX.Blog.Domain
{
    public class ArticleDeleted : IDomainEvent
    {
        public ArticleDeleted(ArticleEntity article)
        {
            Article = article;
        }

        public ArticleEntity Article { get; }
    }
}
