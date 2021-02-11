using LinqSpecs;
using System;
using System.Linq.Expressions;

namespace ProjectX.Blog.Domain
{
    public sealed class ArticlePublicationDateSpecification : Specification<ArticleEntity>
    {
        readonly DateTime _createdAt;

        public ArticlePublicationDateSpecification(DateTime createdAt)
        {
            _createdAt = createdAt;
        }

        public override Expression<Func<ArticleEntity, bool>> ToExpression()
            => a => a.CreatedAt.Date == _createdAt.Date;
    }
}
