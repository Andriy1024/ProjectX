using Marten;
using ProjectX.Core;
using ProjectX.Messenger.Application;
using ProjectX.Messenger.Domain;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.Messenger.Persistence
{
    public sealed class MartenEventStore : IEventStore
    {
        private readonly IDocumentSession _documentSession;

        public MartenEventStore(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public async Task<T> LoadAsync<T>(string id) where T : IEventSourcedAggregate, new()
        {
            Utill.ThrowIfNullOrEmpty(id, nameof(id));

            var events = await _documentSession.Events.FetchStreamAsync(id);

            if (events.Count > 0) return default;

            var aggregate = new T();

            aggregate.Load(events.Select(e => e.Data).Cast<IDomainEvent>());

            return aggregate;
        }

        public async Task StoreAsync<T>(T aggregate) where T : IEventSourcedAggregate
        {
            var uncommitedChanges = aggregate.Changes;

            _documentSession.Events.Append(aggregate.GetId(), aggregate.Version, uncommitedChanges);

            await _documentSession.SaveChangesAsync();
        }
    }
}
