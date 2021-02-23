using ProjectX.Core;

namespace ProjectX.Identity.Application
{
    public class VerifyEmailCommand : ICommand
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
