using Microsoft.AspNetCore.Identity;
using ProjectX.Common;
using System.Collections.Generic;

namespace ProjectX.Identity.Domain
{
    public sealed partial class UserEntity : IdentityUser<long>, IEntity
    {
        private List<IDomainEvent> _domainEvents;
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        public string FirstName { get; protected set; }

        public string LastName { get; protected set; }

        public AddressObject Address { get; protected set; }

        public Builder Factory => new Builder();

        protected UserEntity() {}

        public void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents = _domainEvents ?? new List<IDomainEvent>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(IDomainEvent eventItem) => _domainEvents?.Remove(eventItem);
        
        public void ClearDomainEvents() => _domainEvents?.Clear();
        
        public bool IsTransient() => Id.Equals(default(long));
    }
}
