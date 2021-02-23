using ProjectX.Core;

namespace ProjectX.Identity.Application
{
    public class SendEmailVerificationCommand : ICommand
    {
        public string Email { get; set; }
    }
}
