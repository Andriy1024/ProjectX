﻿using ProjectX.Core;

namespace ProjectX.Messenger.Application
{
    public class SendMessage : ICommand
    {
        public long CompanionId { get; set; }

        public string Content { get; set; }
    }
}