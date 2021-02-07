using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading.Tasks;
using ProjectX.Common.BlackList;
using ProjectX.Common.Infrastructure.Auth;

namespace ProjectX.Common.Infrastructure.BlackList
{
    public sealed class BlackListMiddleware
    {
        readonly RequestDelegate _next;
        readonly ISessionBlackList _sessionBlackList;
        readonly IActionResultExecutor<ObjectResult> _actionResultExecutor;

        public BlackListMiddleware(RequestDelegate next,
            ISessionBlackList sessionBlackList,
            IActionResultExecutor<ObjectResult> actionResultExecutor)
        {
            _next = next;
            _sessionBlackList = sessionBlackList;
            _actionResultExecutor = actionResultExecutor;
        }

        public async Task Invoke(HttpContext context)
        {
            var sessionId = context.User.GetSessionId();
            if (!string.IsNullOrEmpty(sessionId) && await _sessionBlackList.HasSessionAsync(sessionId))
                await _actionResultExecutor.ExecuteAsync(new ActionContext() { HttpContext = context },
                    new UnauthorizedObjectResult(ResponseFactory.InvalidPermission(ErrorCode.SessionInBlackList)));
            else
                await _next.Invoke(context);
        }
    }
}
