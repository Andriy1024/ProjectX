using LinqSpecs;
using ProjectX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ProjectX.Blog.Domain
{
    public sealed class ArticleAuthorSpecefication : Specification<ArticleEntity>
    {
        readonly IEnumerable<long> _authorIds;

        public ArticleAuthorSpecefication(IEnumerable<long> authorIds)
        {
            if (authorIds.IsNullOrEmpty())
                throw new ArgumentException($"{nameof(authorIds)} are empty.");

            _authorIds = authorIds;
        }

        public override Expression<Func<ArticleEntity, bool>> ToExpression()
            => a => _authorIds.Contains(a.AuthorId);
    }
}
