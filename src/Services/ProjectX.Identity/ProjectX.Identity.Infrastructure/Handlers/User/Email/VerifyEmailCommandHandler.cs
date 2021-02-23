using ProjectX.Core;
using ProjectX.Identity.Application;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.Handlers
{
    public sealed class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand>
    {
        private readonly UserManager _userManager;

        public VerifyEmailCommandHandler(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IResponse> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
        {
            var maybeUser = await _userManager.GetUserAsync(u => u.Email == command.Email);

            if (maybeUser.IsFailed)
            {
                return ResponseFactory.Failed(maybeUser.Error);
            }

            var user = maybeUser.Result;

            if (user.EmailConfirmed)
            {
                return ResponseFactory.InvalidData(ErrorCode.EmailAlreadyConfirmed);
            }

            var result = await _userManager.ConfirmEmailAsync(user, Encoding.UTF8.GetString(Convert.FromBase64String(command.Code)));

            if (!result.Succeeded) 
            {
                return ResponseFactory.ServerError(string.Join(", ", result.Errors));
            }
                
            return ResponseFactory.Success();
        }
    }
}
