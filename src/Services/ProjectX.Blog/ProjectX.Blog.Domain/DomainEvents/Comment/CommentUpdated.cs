using ProjectX.Core;

namespace ProjectX.Blog.Domain
{
    public sealed class CommentUpdated : IDomainEvent
    {
        public CommentUpdated(CommentEntity comment)
        {
            Comment = comment;
        }

        public CommentEntity Comment { get; }
    }
}
