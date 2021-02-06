using ProjectX.Common;
using System;
using System.Collections.Generic;

namespace ProjectX.Identity.Domain
{
    public sealed class SessionExpiration : ValueObject
    {
        /// <summary>
        /// One hour
        /// </summary>
        public static int AccessTokenLifetime = 3600;
        
        /// <summary>
        /// 15 days
        /// </summary>
        public static int RefreshTokenLifetime = 1296000;

        public static SessionExpiration Create(DateTime dateTime) => new SessionExpiration() 
        {
            AccessTokenExpiresAt = dateTime.AddSeconds(AccessTokenLifetime),
            RefreshTokenExpiresAt = dateTime.AddSeconds(RefreshTokenLifetime)
        };

        private SessionExpiration() {}

        public DateTime AccessTokenExpiresAt { get; private set; }
        public DateTime RefreshTokenExpiresAt { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return AccessTokenExpiresAt;
            yield return RefreshTokenExpiresAt;
        }
    }
}
