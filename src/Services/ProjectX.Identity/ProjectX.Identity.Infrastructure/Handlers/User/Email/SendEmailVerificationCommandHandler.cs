using ProjectX.Core;
using ProjectX.Email;
using ProjectX.Identity.Application;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.Handlers.User.Email
{
    public sealed class SendEmailVerificationCommandHandler : ICommandHandler<SendEmailVerificationCommand>
    {
        private readonly UserManager _userManager;
        private readonly IEmailSender _emailSender;

        public SendEmailVerificationCommandHandler(UserManager userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IResponse> Handle(SendEmailVerificationCommand command, CancellationToken cancellationToken)
        {
            if (!_emailSender.IsEmailSenderEnabled) 
            {
                return ResponseFactory.InvalidData(ErrorCode.InvalidData, "The email sender is disabled.");
            }
                
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

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var code = Convert.ToBase64String(Encoding.UTF8.GetBytes(confirmationToken));

            var result = await _emailSender.SendAsync(user.Email, "Email verification", body: GetEmailBody(code), senderName: "ProjectX", isHtml: false);

            return result.IsFailed
                    ? ResponseFactory.Failed(result.Error)
                    : ResponseFactory.Success();
        }

        private string GetEmailBody(string code) => $"Your verification code: {code}";
    }
}
