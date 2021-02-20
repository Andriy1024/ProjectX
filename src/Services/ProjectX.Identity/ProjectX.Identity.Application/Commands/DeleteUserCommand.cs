using ProjectX.Core;

namespace ProjectX.Identity.Application
{
    public class DeleteUserCommand : ICommand
    {
        public DeleteUserCommand(long id)
        {
            Id = id;
        }

        public long Id { get; }
    }
}
