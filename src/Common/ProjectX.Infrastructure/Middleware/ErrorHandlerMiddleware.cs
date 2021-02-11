using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using ProjectX.Core;
using ProjectX.Core.Exceptions;
using ProjectX.Infrastructure.REST;
using System;
using System.Threading.Tasks;

namespace ProjectX.Infrastructure.Middleware
{
    /// <summary>
    /// Represents Error handling middleware. Used to process error's user messages.
    /// </summary>
    public sealed class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IActionResultExecutor<ObjectResult> _actionResultExecutor;

        public ErrorHandlerMiddleware(RequestDelegate next,
            ILogger<ErrorHandlerMiddleware> logger,
            IActionResultExecutor<ObjectResult> actionResultExecutor)
        {
            _next = next;
            _logger = logger;
            _actionResultExecutor = actionResultExecutor;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (InvalidDataException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                await SendResponseAsync(context, new BadRequestObjectResult(ResponseFactory.Failed(ex.Error)));
            }
            catch (JsonPatchException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                await SendResponseAsync(context, new BadRequestObjectResult(ResponseFactory.InvalidData(ErrorCode.InvalidData, ex.Message)));
            }
            catch (InvalidPermissionException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                await SendResponseAsync(context, new ForbiddenObjectResult(ResponseFactory.Failed(ex.Error)));
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                await SendResponseAsync(context, new NotFoundObjectResult(ResponseFactory.Failed(ex.Error)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception message: {ex.Message}.\nInner exception: {ex.InnerException?.Message}.\nStack trace: {ex.StackTrace}.");
                await SendResponseAsync(context, new InternalServerErrorObjectResult(ResponseFactory.ServerError(ErrorCode.ServerError, ex.Message)));
            }
        }

        /// <summary>
        /// Executes passed action result.
        /// </summary>
        /// <param name="context">HttpContext of current request.</param>
        /// <param name="objectResult">Instance of ObjectResult implementation, contains error data.</param>
        private Task SendResponseAsync(HttpContext context, ObjectResult objectResult) =>
                _actionResultExecutor.ExecuteAsync(new ActionContext() { HttpContext = context }, objectResult);
    }
}
