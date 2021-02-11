using ProjectX.Core;
using System;
using System.Collections.Generic;

namespace ProjectX.Blog.Domain
{
    public sealed class ArticleSpecificationBuilder : SpecificationBuilder<ArticleEntity>
    {
        public ArticleSpecificationBuilder WhereAuthor(IEnumerable<long> ids) 
        {
            if (!ids.IsNullOrEmpty()) 
            {
                SetSpecification(new ArticleAuthorSpecefication(ids));
            }
            return this;
        }

        public ArticleSpecificationBuilder Search(string search) 
        {
            if (string.IsNullOrEmpty(search)) 
            {
                SetSpecification(new ArticleSearchSpecefication(search));
            }
            return this;
        }

        public ArticleSpecificationBuilder PublishedAt(DateTime? date) 
        {
            if (date.HasValue) 
            {
                SetSpecification(new ArticlePublicationDateSpecification(date.Value));
            }
            return this;
        }
    }
}
