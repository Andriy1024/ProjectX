using ProjectX.Core;
using System;

namespace ProjectX.Identity.Domain
{
    public partial class UserEntity
    {
        public sealed class Builder : EntityBuilder<UserEntity>
        {
            public Builder() {}

            public Builder Email(string email) 
            {
                Utill.ThrowIfNullOrEmpty(email, nameof(email));
                EnsureCreated();
                Entity.Email = email;
                Entity.UserName = email;
                return this;
            }

            public Builder Name(string firstName, string lastName) 
            {
                Utill.ThrowIfNullOrEmpty(firstName, nameof(firstName));
                Utill.ThrowIfNullOrEmpty(lastName, nameof(lastName));
                EnsureCreated();
                Entity.FirstName = firstName;
                Entity.LastName = lastName;
                return this;
            }

            public Builder Address(Address address) 
            {
                Utill.ThrowIfNull(address, nameof(address));
                EnsureCreated();
                Entity.Address = address;
                return this;
            }

            public Builder Role(RoleEntity role)
            {
                Utill.ThrowIfNull(role, nameof(role));
                EnsureCreated();
                new UserRoleEntity(Entity, role);
                return this;
            }

            protected override void EnsureCreated() => Entity ??= new UserEntity();

            public override UserEntity Build() 
            {
                if (Entity == null || Entity.Address == null || Entity.FirstName == null || Entity.Email == null)
                    throw new InvalidOperationException("User Entity not fully initialized.");

                Entity.AddDomainEvent(new UserCreatedDomainEvent(Entity));
                return base.Build(); ;
            }
        }
    }
}
