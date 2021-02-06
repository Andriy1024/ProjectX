using Microsoft.AspNetCore.Identity;
using ProjectX.Identity.Domain;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProjectX.Identity.Persistence;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectX.Common.Exceptions;
using ProjectX.Common;
using System.Threading;
using System.Linq;

namespace ProjectX.Identity.Infrastructure
{
    public sealed class UserManager : UserManager<UserEntity>
    {
        readonly IdentityDbContext _dbContext;

        public UserManager(IUserStore<UserEntity> store,
           IOptions<IdentityOptions> optionsAccessor,
           IPasswordHasher<UserEntity> passwordHasher,
           IEnumerable<IUserValidator<UserEntity>> userValidators,
           IEnumerable<IPasswordValidator<UserEntity>> passwordValidators,
           ILookupNormalizer keyNormalizer,
           IdentityErrorDescriber errors,
           IServiceProvider services,
           ILogger<UserManager<UserEntity>> logger,
           IdentityDbContext dbContext)
           : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
           {
                _dbContext = dbContext;
           }


        #region Session actions

        public async Task<SessionEntity> UpdateSessionAsync(UserEntity user, string sessionId, DateTime dateTime)
        {
            var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session == null)
                throw new NotFoundException(ErrorCode.SessionNotFound);

            if (session.IsActive)
                session.Refresh(dateTime);
            else
                session = user.CreateSession(Guid.NewGuid(), dateTime);

            await _dbContext.SaveChangesAsync();
            return session;
        }

        public async Task<SessionEntity> CreateSessionAsync(UserEntity user, DateTime dateTime)
        {
            var session = user.CreateSession(Guid.NewGuid(), dateTime);
            await _dbContext.SaveChangesAsync();
            return session;
        }

        public async Task PutActiveSessionsInBlackListAsync(long userId, CancellationToken cancellationToken)
        {
            var expireDate = DateTime.UtcNow.AddSeconds(SessionExpiration.RefreshTokenLifetime);
            var sessions = await _dbContext.Sessions.Where(s => s.UserId == userId && s.Expiration.RefreshTokenExpiresAt <= expireDate).ToArrayAsync(cancellationToken);
            if (sessions.Length == 0)
                return;

            var timeSpan = TimeSpan.FromSeconds(SessionExpiration.RefreshTokenLifetime);
            foreach (var session in sessions)
            {
                session.Deactivate(DateTime.UtcNow);
#warning TO DO:
                //await _blackList.AddToBlackListAsync(session.Id, timeSpan);
            }
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
