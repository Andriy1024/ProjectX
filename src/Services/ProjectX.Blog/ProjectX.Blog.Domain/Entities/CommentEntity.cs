using ProjectX.Core;
using System;

namespace ProjectX.Blog.Domain
{
    public sealed class CommentEntity : Entity<long>
    {
        public long ArticleId { get; private set; }
        public ArticleEntity Article { get; private set; }

        public long AuthorId { get; private set; }
        public AuthorEntity Author { get; private set; }

        public string Text { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
    }
}
