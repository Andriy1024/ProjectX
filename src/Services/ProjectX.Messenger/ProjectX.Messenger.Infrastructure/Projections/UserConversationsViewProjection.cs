using Marten.Events.Projections;
using ProjectX.Messenger.Domain;
using System.Linq;

namespace ProjectX.Messenger.Infrastructure.Projections
{
    public class UserConversationsViewProjection : ViewProjection<UserConversationsView, long>
    {
        public UserConversationsViewProjection()
        {
            ProjectEvent<MessageCreated>((ev) => ev.Users.ToList(), (view, @event) => view.Apply(@event));
            ProjectEvent<MessageUpdated>((ev) => ev.Users.ToList(), (view, @event) => view.Apply(@event));
        }
    }
}
