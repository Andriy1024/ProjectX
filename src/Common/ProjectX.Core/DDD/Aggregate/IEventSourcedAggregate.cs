using System.Collections.Generic;

namespace ProjectX.Core
{
    public interface IEventSourcedAggregate
    {
        string GetId();
        int Version { get; }
        IReadOnlyCollection<IDomainEvent> Changes { get; }
        void ClearChanges();
        void Load(IEnumerable<IDomainEvent> events);
    }
}
