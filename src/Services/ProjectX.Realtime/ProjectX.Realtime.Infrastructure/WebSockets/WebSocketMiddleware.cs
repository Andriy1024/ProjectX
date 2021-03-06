using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ProjectX.Core;
using ProjectX.Core.Auth;
using ProjectX.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Realtime.Infrastructure
{
    public sealed class WebSocketMiddleware
    {
        private const string ConnectionIdQueryParamName = "connectionId";
        private const string CookieTokenParamName = "token";

        private readonly WebSocketConnectionManager _connectionManager;
        private readonly ITokenProvider _tokenProvider;

        public WebSocketMiddleware(RequestDelegate next,
            WebSocketConnectionManager connectionManager,
            ITokenProvider tokenProvider)
        {
            _connectionManager = connectionManager;
            _tokenProvider = tokenProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest) return;

            string connectionId;
            long userId;

            var cancellationToken = context.RequestAborted;

            if (TryGetCookieTokenParam(context, out string accessToken))
            {
                var isValid = await ValidateAccessTokenAsync(accessToken, cancellationToken);
                if (!isValid)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }

                userId = GetUserId(accessToken);
                if (userId == default)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }

                connectionId = Guid.NewGuid().ToString();
            }
            else
            {
                if (!TryGetConnectionIdQueryParam(context, out connectionId))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }

                if (!_connectionManager.TryGetUserId(connectionId, out userId))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return;
                }
            }

            var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            await _connectionManager.ConnectAsync(connectionId, userId, webSocket, cancellationToken);
        }

        private bool TryGetConnectionIdQueryParam(HttpContext context, out string connectionId)
        {
            connectionId = null;
            var connectionIdParameter = context.Request.Query.FirstOrDefault(t =>
                string.Equals(t.Key,
                    ConnectionIdQueryParamName,
                    StringComparison.CurrentCultureIgnoreCase));

            if (!connectionIdParameter.Equals(default(KeyValuePair<string, StringValues>)))
            {
                connectionId = connectionIdParameter.Value;
            }

            return !string.IsNullOrEmpty(connectionId);
        }

        private bool TryGetCookieTokenParam(HttpContext context, out string token)
        {
            context.Request.Cookies.TryGetValue(CookieTokenParamName, out token);

            return !string.IsNullOrEmpty(token);
        }

        private Task<bool> ValidateAccessTokenAsync(string accessToken, CancellationToken cancellationToken)
        {
            return _tokenProvider.IntrospectTokenAsync(accessToken, cancellationToken);
        }

        private long GetUserId(string token)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            
            var jwtToken = jwtTokenHandler.ReadToken(token) as JwtSecurityToken;

            var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value;

            if (string.IsNullOrEmpty(userId) || !long.TryParse(userId, out var validId)) 
            {
                throw new InvalidDataException(ErrorCode.NoIdentityIdInAccessToken);
            }

            return validId;
        }
    }
}
