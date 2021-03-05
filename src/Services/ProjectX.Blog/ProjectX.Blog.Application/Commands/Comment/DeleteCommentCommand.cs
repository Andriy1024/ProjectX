using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class DeleteCommentCommand : ICommand
    {
        public DeleteCommentCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}
