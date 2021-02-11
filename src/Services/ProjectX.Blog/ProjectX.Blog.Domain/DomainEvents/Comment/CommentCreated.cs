using ProjectX.Core;

namespace ProjectX.Blog.Domain
{
    public sealed class CommentCreated : IDomainEvent
    {
        public CommentCreated(CommentEntity comment)
        {
            Comment = comment;
        }

        public CommentEntity Comment { get; }
    }
}
