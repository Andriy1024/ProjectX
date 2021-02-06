using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using ProjectX.Common;
using ProjectX.Identity.Persistence.IdentityServer;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Identity.Persistence.Startup
{
    public class IdentityServerStartupTask : IStartupTask
    {
        readonly ConfigurationDbContext _configurationDb;
        readonly PersistedGrantDbContext _persistedGrantDb;

        public IdentityServerStartupTask(ConfigurationDbContext configurationDb, PersistedGrantDbContext persistedGrantDb)
        {
            _configurationDb = configurationDb;
            _persistedGrantDb = persistedGrantDb;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            _configurationDb.Database.Migrate();
            _persistedGrantDb.Database.Migrate();

            var clients = await _configurationDb.Clients.Include(t => t.AllowedScopes).ToDictionaryAsync(t => t.ClientId, t => t);
            foreach (var config in IdentityServerConfig.GetClients())
            {
                if (clients.TryGetValue(config.ClientId, out var entity))
                {
                    ApllyChanges(entity, config);
                }
                else
                {
                    await _configurationDb.Clients.AddAsync(config.ToEntity());
                }
            }

            if (!await _configurationDb.IdentityResources.AnyAsync())
            {
                foreach (var resource in IdentityServerConfig.GetIdentityResources())
                {
                    await _configurationDb.IdentityResources.AddAsync(resource.ToEntity());
                }
            }

            var apiResources = await _configurationDb.ApiResources.ToDictionaryAsync(t => t.Name, t => t);
            foreach (var config in IdentityServerConfig.GetApis())
            {
                if (!apiResources.TryGetValue(config.Name, out var _))
                {
                    await _configurationDb.ApiResources.AddAsync(config.ToEntity());
                }
            }

            await _configurationDb.SaveChangesAsync();
        }

        public void ApllyChanges(IdentityServer4.EntityFramework.Entities.Client entity, IdentityServer4.Models.Client config)
        {
            var newScopes = config.AllowedScopes.Except(entity.AllowedScopes.Select(t => t.Scope));
            if (newScopes.Any())
            {
                entity.AllowedScopes.AddRange(newScopes.Select(t => new IdentityServer4.EntityFramework.Entities.ClientScope()
                {
                    Scope = t,
                    ClientId = entity.Id
                }));
            }

            entity.AccessTokenLifetime = config.AccessTokenLifetime;
            entity.UpdateAccessTokenClaimsOnRefresh = config.UpdateAccessTokenClaimsOnRefresh;
            entity.SlidingRefreshTokenLifetime = config.SlidingRefreshTokenLifetime;
            entity.RefreshTokenExpiration = (int)config.RefreshTokenExpiration;
            entity.RefreshTokenUsage = (int)config.RefreshTokenUsage;
        }
    }
}
