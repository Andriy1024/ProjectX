using LinqSpecs;
using ProjectX.Core;
using System;
using System.Linq.Expressions;

namespace ProjectX.Blog.Domain
{
    public sealed class ArticleSearchSpecefication : Specification<ArticleEntity>
    {
        readonly string _search;

        public ArticleSearchSpecefication(string search)
        {
            Utill.ThrowIfNullOrEmpty(search, nameof(search));
            _search = search;
        }

        public override Expression<Func<ArticleEntity, bool>> ToExpression()
            => a => a.Title.Contains(_search);
    }
}
