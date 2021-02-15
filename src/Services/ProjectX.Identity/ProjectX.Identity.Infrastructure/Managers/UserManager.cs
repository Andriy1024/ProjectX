using Microsoft.AspNetCore.Identity;
using ProjectX.Identity.Domain;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectX.Identity.Persistence;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectX.Core.Exceptions;
using ProjectX.Core;
using System.Threading;
using System.Linq;
using ProjectX.Core.BlackList;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace ProjectX.Identity.Infrastructure
{
    public sealed class UserManager : UserManager<UserEntity>
    {
        public readonly IdentityDbContext DbContext;
        private readonly Lazy<ISessionBlackList> _blackList;

        public UserManager(IUserStore<UserEntity> store,
           IOptions<IdentityOptions> optionsAccessor,
           IPasswordHasher<UserEntity> passwordHasher,
           IEnumerable<IUserValidator<UserEntity>> userValidators,
           IEnumerable<IPasswordValidator<UserEntity>> passwordValidators,
           ILookupNormalizer keyNormalizer,
           IdentityErrorDescriber errors,
           IServiceProvider services,
           ILogger<UserManager<UserEntity>> logger,
           IdentityDbContext dbContext,
           IServiceProvider serviceProvider)
           : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            DbContext = dbContext;
            _blackList = new Lazy<ISessionBlackList>(() => serviceProvider.GetRequiredService<ISessionBlackList>());
        }

        public async Task<ResultOf<UserEntity>> GetUserWithRolesAsync(Expression<Func<UserEntity, bool>> expression, CancellationToken cancellationToken = default)
        {
            var user = await DbContext.Users.Include(u => u.UserRoles)
                                            .ThenInclude(r => r.Role)
                                            .FirstOrDefaultAsync(expression, cancellationToken);

            if (user == null)
                return Error.NotFound(ErrorCode.UserNotFound);

            return user;
        }

        #region Session actions

        public async Task<SessionEntity> UpdateSessionAsync(UserEntity user, string sessionId, DateTime dateTime)
        {
            var session = await DbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session == null)
                throw new NotFoundException(ErrorCode.SessionNotFound);

            if (session.IsActive)
                session.RefreshLifetime(dateTime);
            else
                session = user.CreateSession(Guid.NewGuid(), dateTime);

            await DbContext.SaveChangesAsync();
            return session;
        }

        public async Task<SessionEntity> CreateSessionAsync(UserEntity user, DateTime dateTime)
        {
            var session = user.CreateSession(Guid.NewGuid(), dateTime);
            await DbContext.SaveChangesAsync();
            return session;
        }

        public async Task PutActiveSessionsInBlackListAsync(long userId, CancellationToken cancellationToken)
        {
            var expireDate = DateTime.UtcNow.AddSeconds(SessionLifetime.AccessTokenLifetime);
            var sessions = await DbContext.Sessions.Where(s => s.UserId == userId && s.Lifetime.AccessTokenExpiresAt <= expireDate).ToArrayAsync(cancellationToken);
            if (sessions.Length == 0)
                return;

            var timeSpan = TimeSpan.FromSeconds(SessionLifetime.AccessTokenLifetime);
            foreach (var session in sessions)
            {
                session.Deactivate(DateTime.UtcNow);
                await _blackList.Value.AddToBlackListAsync(session.Id, timeSpan);
            }
            await DbContext.SaveChangesAsync();
        }

        #endregion
    }
}
