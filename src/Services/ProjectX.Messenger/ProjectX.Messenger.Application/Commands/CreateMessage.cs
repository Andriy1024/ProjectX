using ProjectX.Core;

namespace ProjectX.Messenger.Application
{
    public class SendMessage : ICommand
    {
        public long Recipient { get; set; }

        public string Content { get; set; }
    }
}
