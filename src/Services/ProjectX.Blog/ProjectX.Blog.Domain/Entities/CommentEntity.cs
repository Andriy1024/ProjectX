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
        public byte[] RowVersion { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private CommentEntity() { }

        public CommentEntity(AuthorEntity author, ArticleEntity article, string text)
        {
            Utill.ThrowIfNull(author, nameof(author));
            Utill.ThrowIfNull(article, nameof(article));

            Author = author;
            AuthorId = author.Id;
            Article = article;
            ArticleId = article.Id;
            Text = text;
            CreatedAt = UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new CommentCreated(this));
        }

        public void Delete() 
        {
            AddDomainEvent(new CommentDeleted(this));
        }

        public void Update(string text) 
        {
            Text = text;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new CommentUpdated(this));
        }
    }
}
