using Microsoft.AspNetCore.Identity;

namespace ProjectX.Identity.Domain
{
    public class RoleEntity : IdentityRole<long>
    {
        private RoleEntity() { }

        public RoleEntity(string roleName) : base(roleName)
        {
        }
    }
}
