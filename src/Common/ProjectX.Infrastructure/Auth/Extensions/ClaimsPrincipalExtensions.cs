using ProjectX.Core;
using ProjectX.Core.Auth;
using ProjectX.Core.Exceptions;
using System.Linq;
using System.Security.Claims;

namespace ProjectX.Infrastructure.Auth
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetIdentityId(this ClaimsPrincipal user)
        {
            var stringId = user.Claims.FirstOrDefault(c => c.Type == ClaimType.IdentityId)?.Value;

            if (!string.IsNullOrEmpty(stringId) && long.TryParse(stringId, out long value)) 
            {
                return value;
            }

            throw new InvalidDataException(ErrorCode.NoIdentityIdInAccessToken);
        }

        public static string GetIdentityRole(this ClaimsPrincipal user)
            => user.Claims.FirstOrDefault(c => c.Type == ClaimType.IdentityRole)?.Value
                   ?? throw new InvalidDataException(ErrorCode.NoIdentityRoleInAccessToken);
        
        public static string GetSessionId(this ClaimsPrincipal user)
            => user.FindFirst(ClaimType.Session)?.Value;
    }
}
