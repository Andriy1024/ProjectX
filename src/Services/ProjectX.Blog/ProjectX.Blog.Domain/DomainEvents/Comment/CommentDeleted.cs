using ProjectX.Core;

namespace ProjectX.Blog.Domain
{
    public sealed class CommentDeleted : IDomainEvent
    {
        public CommentDeleted(CommentEntity comment)
        {
            Comment = comment;
        }

        public CommentEntity Comment { get; }
    }
}
