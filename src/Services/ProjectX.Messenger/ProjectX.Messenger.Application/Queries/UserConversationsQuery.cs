using ProjectX.Core;
using ProjectX.Messenger.Domain;
using System.Collections.Generic;

namespace ProjectX.Messenger.Application
{
    public sealed class UserConversationsQuery : IQuery<List<UserConversationsView.Conversation>>
    {
    }
}
