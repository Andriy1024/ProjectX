using Marten;
using MediatR;
using ProjectX.Core;
using ProjectX.DataAccess;
using ProjectX.Messenger.Application;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.Messenger.Persistence
{
    public sealed class MartenEventStore : IEventStore
    {
        private readonly IDocumentSession _documentSession;
        private readonly IMediator _mediator;

        public MartenEventStore(IDocumentSession documentSession, IMediator mediator)
        {
            _documentSession = documentSession;
            _mediator = mediator;
        }

        public async Task<T> LoadAsync<T>(string id) where T : IEventSourcedAggregate, new()
        {
            Utill.ThrowIfNullOrEmpty(id, nameof(id));

            var events = await _documentSession.Events.FetchStreamAsync(id);

            if (events.Count == 0) return default;

            var aggregate = new T();

            aggregate.Load(events.Select(e => e.Data).Cast<IDomainEvent>());

            return aggregate;
        }

        public async Task StoreAsync<T>(T aggregate) where T : IEventSourcedAggregate
        {
            var uncommitedChanges = aggregate.Changes;

            _documentSession.Events.Append(aggregate.GetId(), aggregate.Version, uncommitedChanges);

            foreach (var @event in uncommitedChanges)
            {
                await _mediator.Publish(uncommitedChanges);
            }

            await _documentSession.SaveChangesAsync();
            
            await _mediator.Publish(new TransactionCommitedEvent());
        }
    }
}
