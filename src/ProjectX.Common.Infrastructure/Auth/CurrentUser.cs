using Microsoft.AspNetCore.Http;
using ProjectX.Common.Auth;

namespace ProjectX.Common.Infrastructure.Auth
{
    public class CurrentUser : ICurrentUser
    {
        long? _identityId;
        string _identityRole;

        readonly IHttpContextAccessor _contextAccessor;

        public CurrentUser(IHttpContextAccessor contextAccessor) => _contextAccessor = contextAccessor;

        public long IdentityId
        {
            get
            {
                if (!_identityId.HasValue)
                     _identityId = _contextAccessor.HttpContext.User.GetIdentityId();

                return _identityId.Value;
            }
        }

        public string IdentityRole
        {
            get
            {
                if (string.IsNullOrEmpty(_identityRole))
                    _identityRole = _contextAccessor.HttpContext.User.GetIdentityRole();

                return _identityRole;
            }
        }
    }
}
