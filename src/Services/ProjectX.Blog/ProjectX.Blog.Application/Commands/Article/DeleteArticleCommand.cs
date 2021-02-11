using ProjectX.Core;

namespace ProjectX.Blog.Application
{
    public sealed class DeleteArticleCommand : ICommand
    {
        public DeleteArticleCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}
