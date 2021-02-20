using ProjectX.Core;
using ProjectX.Identity.Application;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.Handlers
{
    public class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        readonly UserManager _userManager;

        public DeleteUserCommandHandler(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IResponse> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
        {
            var result = await _userManager.GetUserWithRolesAsync(u => u.Id == command.Id, cancellationToken);
            if (result.IsFailed)
                return ResponseFactory.Failed(result.Error);

            await _userManager.RemoveAsync(result.Result, cancellationToken);

            return Response.Success;
        }
    }
}
