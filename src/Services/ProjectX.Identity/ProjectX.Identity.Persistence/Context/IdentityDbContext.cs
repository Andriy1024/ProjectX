using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectX.Core;
using ProjectX.Identity.Domain;
using ProjectX.Identity.Persistence.EntityConfigurations;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        private readonly IMediator _mediator;

        public const string SchemaName = "ProjectX.Identity";

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(SchemaName);
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new SessionConfiguration());
            builder.ApplyConfiguration(new UserRoleConfiguration());
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEntities = ChangeTracker
                                .Entries<IEntity>()
                                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Count > 0)
                                .ToArray();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToArray();

            for (int i = 0; i < domainEntities.Length; i++)
                domainEntities[i].Entity.ClearDomainEvents();

            var result = await base.SaveChangesAsync(cancellationToken);

            for (int i = 0; i < domainEvents.Length; i++)
                await _mediator.Publish(domainEvents[i]);

            return result;
        }
    }
}
