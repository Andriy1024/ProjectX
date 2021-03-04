using ProjectX.Core;
using ProjectX.Messenger.Application.Views;

namespace ProjectX.Messenger.Application
{
    public class GetConversationViewQuery : IQuery<ConversationView>
    {
        public long CompanionId { get; set; }
    }
}
