using System.Collections.Generic;

namespace ProjectX.Core
{
    public abstract class EventSourcedAggregate : IEventSourcedAggregate
    {
        /// <summary>
        /// Changes that should be committed to an event store.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> Changes => _changes;

        private readonly List<IDomainEvent> _changes = new List<IDomainEvent>();

        public abstract string GetId();

        /// <summary>
        /// The action should be triggered after changes committed to an event store.
        /// </summary>
        public void ClearChanges() => _changes.Clear();

        /// <summary>
        /// Version represents the number of commits events.
        /// </summary>
        public int Version { get; private set; }

        protected abstract void When(IDomainEvent @event);

        protected void Apply(IDomainEvent evt)
        {
            _changes.Add(evt);
            When(evt);
            Version++;
        }

        public void Load(IEnumerable<IDomainEvent> events)
        {
            foreach (var @event in events)
            {
                When(@event);
                Version++;
            }
        }
    }
}
