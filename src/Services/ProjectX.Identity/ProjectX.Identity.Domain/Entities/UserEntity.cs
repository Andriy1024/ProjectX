using Microsoft.AspNetCore.Identity;
using ProjectX.Common;
using System;
using System.Collections.Generic;

namespace ProjectX.Identity.Domain
{
    public sealed partial class UserEntity : IdentityUser<long>, IEntity
    {
        private List<IDomainEvent> _domainEvents;
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        public string FirstName { get; protected set; }

        public string LastName { get; protected set; }

        public Address Address { get; protected set; }

        public ICollection<SessionEntity> Sessions { get; private set; } = new List<SessionEntity>();

        public static Builder Factory => new Builder();

        protected UserEntity() {}

        public void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents = _domainEvents ?? new List<IDomainEvent>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(IDomainEvent eventItem) => _domainEvents?.Remove(eventItem);
        
        public void ClearDomainEvents() => _domainEvents?.Clear();
        
        public bool IsTransient() => Id.Equals(default(long));

        public void SetName(string firstName, string lastName) 
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public SessionEntity CreateSession(Guid id, DateTime createdAt)
        {
            var session = new SessionEntity(this, id, createdAt);
            return session;
        }
    }
}
