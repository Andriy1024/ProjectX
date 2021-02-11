using ProjectX.Core;
using System;
using System.Collections.Generic;

namespace ProjectX.Blog.Domain
{
    public sealed partial class ArticleEntity : Entity<long>
    {
        public long AuthorId { get; private set; }
        public AuthorEntity Author { get; private set; }
        public string Title { get; private set; }
        public string Body { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public byte[] RowVersion { get; private set; }

        private readonly List<CommentEntity> _comments = new List<CommentEntity>();
        public IReadOnlyCollection<CommentEntity> Comments => _comments;

        public static Builder Factory => new Builder();
        public static ArticleSpecificationBuilder SpeceficationFactory => new ArticleSpecificationBuilder();

        protected ArticleEntity() {}

        public void Delete() 
        {
            AddDomainEvent(new ArticleDeleted(this));
        }

        public void UpdateTitle(string title) 
        {
            Title = title;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ArticleTittleUpdated(this));
        }

        public void UpdateBody(string body)
        {
            Body = body;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new ArticleBodyUpdated(this));
        }
    }
}
