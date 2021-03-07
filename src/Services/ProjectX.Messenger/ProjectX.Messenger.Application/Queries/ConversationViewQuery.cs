using ProjectX.Core;
using ProjectX.Messenger.Domain;

namespace ProjectX.Messenger.Application
{
    public sealed class ConversationViewQuery : IQuery<ConversationView>
    {
        /// <summary>
        /// Id of conversation's member.
        /// </summary>
        public long UserId { get; set; }
    }
}
