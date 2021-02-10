using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ProjectX.Identity.Domain
{
    public sealed class RoleEntity : IdentityRole<long>
    {
        public ICollection<UserRoleEntity> UserRoles { get; private set; } = new List<UserRoleEntity>();

        private RoleEntity() { }

        public RoleEntity(string roleName) : base(roleName)
        {
        }
    }
}
