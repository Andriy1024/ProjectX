using ProjectX.Core;
using ProjectX.Core.Exceptions;
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

        public CommentEntity(long authorId, long articleId, string text)
        {
            AuthorId = authorId;
            ArticleId = articleId;
            Text = text;
            CreatedAt = UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new CommentCreated(this));
        }

        public void Delete(long deleteBy) 
        {
            if(deleteBy != AuthorId) 
            {
                throw new InvalidPermissionException(ErrorCode.InvalidPermission, "Comment can be deleted only by the owner.");
            }

            AddDomainEvent(new CommentDeleted(this));
        }

        public void Update(string text, long updateBy) 
        {
            if (updateBy != AuthorId)
            {
                throw new InvalidPermissionException(ErrorCode.InvalidPermission, "Comment can be updated only by the owner.");
            }

            Text = text;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new CommentUpdated(this));
        }
    }
}
