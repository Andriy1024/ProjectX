using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectX.Identity.Domain;
using ProjectX.Identity.Persistence.EntityConfigurations;

namespace ProjectX.Identity.Persistence
{
    public sealed class IdentityDbContext : IdentityDbContext<UserEntity, RoleEntity, long, IdentityUserClaim<long>,
            UserRoleEntity, IdentityUserLogin<long>,
            IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public override DbSet<UserEntity> Users { get; set; }
        public override DbSet<RoleEntity> Roles { get; set; }
        public override DbSet<UserRoleEntity> UserRoles { get; set; }
        public DbSet<SessionEntity> Sessions { get; set; }

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new SessionConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
        }
    }
}
