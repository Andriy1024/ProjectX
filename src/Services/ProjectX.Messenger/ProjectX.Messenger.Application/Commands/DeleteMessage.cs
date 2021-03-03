using ProjectX.Core;
using System;

namespace ProjectX.Messenger.Application
{
    public class DeleteMessage : ICommand
    {
        public string ConversationId { get; set; }

        public Guid MessageId { get; set; }
    }
}
