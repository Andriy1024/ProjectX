using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using ProjectX.Core.Auth;
using ProjectX.Core.Cache;
using ProjectX.Infrastructure.Auth;
using ProjectX.Identity.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProjectX.Identity.Infrastructure.Managers
{
    public class ProfileManager : IProfileService
    {
        readonly UserManager _userManager;
        readonly IScopedCache<string, UserEntity> _cache;

        public ProfileManager(UserManager userManager, IScopedCache<string, UserEntity> cache)
        {
            _userManager = userManager;
            _cache = cache;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = _cache[sub];
            var sessionId = await GetSessionAsync(user, context.Subject);
            var roles = await _userManager.GetRolesAsync(user);
            
            var tokenClaims = new List<Claim>();
            tokenClaims.AddRange(roles.Select(role => new Claim(ClaimType.IdentityRole, role)).ToList());
            tokenClaims.Add(new Claim(ClaimType.Username, user.UserName));
            tokenClaims.Add(new Claim(ClaimType.Session, sessionId));

            context.IssuedClaims.AddRange(tokenClaims);
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }

        private async Task<string> GetSessionAsync(UserEntity user, ClaimsPrincipal claims)
        {
            var sessionId = claims.GetSessionId();
            var session = string.IsNullOrEmpty(sessionId)
                        ? await _userManager.CreateSessionAsync(user, DateTime.UtcNow)
                        : await _userManager.UpdateSessionAsync(user, sessionId, DateTime.UtcNow);

            return session.Id;
        }
    }
}
