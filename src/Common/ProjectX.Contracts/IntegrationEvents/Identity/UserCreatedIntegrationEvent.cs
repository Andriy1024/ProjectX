using ProjectX.Core.IntegrationEvents;

namespace ProjectX.Contracts.IntegrationEvents
{
    public class UserCreatedIntegrationEvent : IIntegrationEvent
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public UserCreatedIntegrationEvent()
        {

        }

        public UserCreatedIntegrationEvent(long id, string firstName, string lastName, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
    }
}
