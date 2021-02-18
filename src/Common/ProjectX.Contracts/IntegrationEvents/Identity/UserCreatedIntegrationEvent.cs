using ProjectX.Core.IntegrationEvents;
using System;

namespace ProjectX.Contracts.IntegrationEvents
{
    public class UserCreatedIntegrationEvent : IIntegrationEvent
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        public UserCreatedIntegrationEvent()
        {
        }

        public UserCreatedIntegrationEvent(Guid id, long userId, string firstName, string lastName, string email)
        {
            Id = id;
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}
