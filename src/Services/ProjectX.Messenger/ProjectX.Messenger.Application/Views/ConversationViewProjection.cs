using Marten.Events.Projections;
using ProjectX.Messenger.Domain;

namespace ProjectX.Messenger.Application.Views
{
    public class ConversationViewProjection : ViewProjection<ConversationView, string>
    {
        public ConversationViewProjection()
        {
            ProjectEvent<ConversationStarted>((ev) => ev.Id, (view, @event) => view.Apply(@event));
            ProjectEvent<MessageCreated>((ev) => ev.ConversationId, (view, @event) => view.Apply(@event));
            ProjectEvent<MessageDeleted>((ev) => ev.ConversationId, (view, @event) => view.Apply(@event));
            ProjectEvent<MessageUpdated>((ev) => ev.ConversationId, (view, @event) => view.Apply(@event));
        }
    }
}
