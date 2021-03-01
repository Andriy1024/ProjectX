using ProjectX.Core;
using System;
using System.Collections.Generic;

namespace ProjectX.Messenger.Domain
{
    public interface IEventSourcedAggregate 
    {
        Guid Id { get; }
        int Version { get; }
        IReadOnlyCollection<IDomainEvent> Changes { get; }
        void ClearChanges();
        void Load(IEnumerable<IDomainEvent> events);
    }

    public abstract class EventSourcedAggregate : IEventSourcedAggregate
    {
        /// <summary>
        /// Aggregate id == stream id.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Changes that should be committed to an event store.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> Changes => _changes;

        private readonly List<IDomainEvent> _changes = new List<IDomainEvent>();

        /// <summary>
        /// The action should be triggered after changes committed to an event store.
        /// </summary>
        public void ClearChanges() => _changes.Clear();

        /// <summary>
        /// Version represents the number of commits events.
        /// </summary>
        public int Version { get; private set; }

        protected void When(IDomainEvent @event)
        {
            When((dynamic)@event);
            Version++;
        }

        protected void Apply(IDomainEvent evt)
        {
            _changes.Add(evt);
            When(evt);
        }

        public void Load(IEnumerable<IDomainEvent> events)
        {
            foreach (var @event in events)
            {
                When(@event);
            }
        }
    }
}
