using ProjectX.Core;

namespace ProjectX.Identity.Domain
{
    public sealed class UserCreatedDomainEvent : IDomainEvent
    {
        public UserCreatedDomainEvent(UserEntity user)
        {
            User = user;
        }

        public UserEntity User { get; }
    }
}
