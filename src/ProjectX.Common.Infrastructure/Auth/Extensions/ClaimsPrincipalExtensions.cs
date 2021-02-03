using ProjectX.Common.Auth;
using ProjectX.Common.Exceptions;
using System.Linq;
using System.Security.Claims;

namespace ProjectX.Common.Infrastructure.Auth
{
    public static class ClaimsPrincipalExtensions
    {
        public static long GetIdentityId(this ClaimsPrincipal user)
        {
            if (!long.TryParse(user.Claims.FirstOrDefault(c => c.Type == ClaimType.IdentityId)?.Value, out long value))
                throw new InvalidDataException(ErrorCode.NoIdentityIdInAccessToken);

            return value;
        }

        public static string GetIdentityRole(this ClaimsPrincipal user)
            => user.Claims.FirstOrDefault(c => c.Type == ClaimType.IdentityRole)?.Value
                   ?? throw new InvalidDataException(ErrorCode.NoIdentityRoleInAccessToken);
        
        public static string GetSessionId(this ClaimsPrincipal user)
            => user.FindFirst(ClaimType.Session)?.Value;
    }
}
