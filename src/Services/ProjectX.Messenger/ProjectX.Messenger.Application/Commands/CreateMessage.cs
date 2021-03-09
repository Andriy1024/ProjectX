using ProjectX.Core;

namespace ProjectX.Messenger.Application
{
    public class SendMessage : ICommand
    {
        /// <summary>
        /// Receiver's id.
        /// </summary>
        public long UserId { get; set; }

        public string Content { get; set; }
    }
}
