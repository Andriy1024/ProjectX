using ProjectX.Core;

namespace ProjectX.Identity.Domain
{
    public sealed class UserDeletedDomainEvent : IDomainEvent
    {
        public UserEntity User { get; }

        public UserDeletedDomainEvent(UserEntity user)
        {
            User = user;
        }
    }
}
