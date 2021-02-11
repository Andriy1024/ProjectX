using ProjectX.Core;
using System;
using System.Collections.Generic;

namespace ProjectX.Blog.Domain
{
    public sealed class ArticleEntity : Entity<long>
    {
        public long AuthorId { get; private set; }
        public AuthorEntity Author { get; private set; }
        public string Tittle { get; private set; }
        public string Body { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private List<CommentEntity> _comments = new List<CommentEntity>();
        public IReadOnlyCollection<CommentEntity> Comments => _comments;
    }
}
