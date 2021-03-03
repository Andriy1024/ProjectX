using ProjectX.Core;
using System;

namespace ProjectX.Messenger.Application
{
    public class UpdateMessage : ICommand
    {
        public string ConversationId { get; set; }
        public Guid MessageId { get; set; }
        public string Content { get; set; }
    }
}
