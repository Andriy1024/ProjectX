using Microsoft.AspNetCore.Identity;

namespace ProjectX.Identity.Domain
{
    public sealed class UserRoleEntity : IdentityUserRole<long>
    {
        public UserEntity User { get; private set; }
        public RoleEntity Role { get; private set; }

        public UserRoleEntity()
        {
        }

        public UserRoleEntity(UserEntity user, RoleEntity role)
        {
            User = user;
            Role = role;
            User.UserRoles.Add(this);
            Role.UserRoles.Add(this);
        }
    }
}
