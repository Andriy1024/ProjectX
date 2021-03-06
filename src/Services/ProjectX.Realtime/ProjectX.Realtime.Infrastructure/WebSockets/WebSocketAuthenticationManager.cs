using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using ProjectX.Core.Auth;
using System;

namespace ProjectX.Realtime.Infrastructure
{
    public sealed class WebSocketAuthenticationManager
    {
        private readonly IMemoryCache _pendingConnectionIds;
        private TimeSpan _cacheItemExpirationTime = TimeSpan.FromSeconds(30);

        public WebSocketAuthenticationManager(ILoggerFactory loggerFactory)
        {
            _pendingConnectionIds = new MemoryCache(new MemoryCacheOptions { ExpirationScanFrequency = _cacheItemExpirationTime }, loggerFactory);
        }

        /// <summary>
        /// First step of authentication. 
        /// The action is triggered from RealtimeController.
        /// </summary>
        public ConnectionId GenerateConnectionId(ICurrentUser currentUser)
        {
            ConnectionId connectionId = Guid.NewGuid();

            _pendingConnectionIds.Set(connectionId, currentUser.IdentityId, _cacheItemExpirationTime);

            return connectionId;
        }

        /// <summary>
        /// Second step of authentication. 
        /// The action is triggered from <see cref="WebSocketMiddleware">.
        /// </summary>
        public bool Validate(ConnectionId connectionId, out long userId)
        {
            var exist = _pendingConnectionIds.TryGetValue(connectionId, out userId);

            if (exist) 
            {
                _pendingConnectionIds.Remove(connectionId);
            }
                
            return exist;
        }
    }
}
