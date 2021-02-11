using ProjectX.Core;
using System;
using System.Collections.Generic;

namespace ProjectX.Identity.Domain
{
    public sealed class SessionLifetime : ValueObject
    {
        /// <summary>
        /// One hour
        /// </summary>
        public static int AccessTokenLifetime = 3600;
        
        /// <summary>
        /// 15 days
        /// </summary>
        public static int RefreshTokenLifetime = 1296000;

        public static SessionLifetime Create(DateTime dateTime) => new SessionLifetime() 
        {
            AccessTokenExpiresAt = dateTime.AddSeconds(AccessTokenLifetime),
            RefreshTokenExpiresAt = dateTime.AddSeconds(RefreshTokenLifetime)
        };

        private SessionLifetime() {}

        public DateTime AccessTokenExpiresAt { get; private set; }
        public DateTime RefreshTokenExpiresAt { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return AccessTokenExpiresAt;
            yield return RefreshTokenExpiresAt;
        }
    }
}
