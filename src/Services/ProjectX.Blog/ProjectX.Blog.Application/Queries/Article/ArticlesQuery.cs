using ProjectX.Core;
using ProjectX.Core.SeedWork;
using System;

namespace ProjectX.Blog.Application
{
    public sealed class ArticlesQuery : IQuery<ArticleDto[]>, IPaginationOptions, IOrderingOptions
    {
        public DateTime? PublishedAt { get; set; }

        public long? AuthorId { get; set; }

        public string Search { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; } = 100;

        public string OrderBy { get; set; }

        public bool Descending { get; set; }
    }
}
