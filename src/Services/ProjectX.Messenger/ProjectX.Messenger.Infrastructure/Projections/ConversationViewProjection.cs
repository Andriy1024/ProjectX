using Marten;
using Marten.Events.Projections;
using ProjectX.Messenger.Domain;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.Messenger.Infrastructure.Projections
{
    public class ConversationViewProjection : ViewProjection<ConversationView, string>
    {
        public ConversationViewProjection()
        {
            ProjectEvent<ConversationStarted>((ev) => ev.Id, (view, @event) => view.Apply(@event));
            ProjectEvent<MessageCreated>((ev) => ev.ConversationId, (view, @event) => view.Apply(@event));
            ProjectEvent<MessageUpdated>((ev) => ev.ConversationId, (view, @event) => view.Apply(@event));
            ProjectEventAsync<MessageDeleted>((ev) => ev.ConversationId, Apply);
        }

        public async Task Apply(IDocumentSession session, ConversationView view, MessageDeleted @event) 
        {
            view.Apply(@event);

            var lastMessage = view.Messages.LastOrDefault();

            foreach (var user in view.Users)
            {
                var userConversations = await session.Query<UserConversationsView>().FirstOrDefaultAsync(u => u.UserId == user);

                if (userConversations == null) return;

                if (lastMessage == null) 
                {
                    userConversations.DeleteConversation(view.Id);
                }
                else 
                {
                    userConversations.Apply(lastMessage);
                }

                session.Store(userConversations);
            }
        }
    }
}
