using ProjectX.Infrastructure.DataAccess;

namespace ProjectX.Identity.Persistence.Context
{
    public class IdentityDbContextFactory : DbContextWithMediatRFactory<IdentityDbContext> { }
}
