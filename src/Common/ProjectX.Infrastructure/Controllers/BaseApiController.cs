using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Infrastructure.REST;
using ProjectX.Core;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace ProjectX.Infrastructure.Controllers
{
    /// <summary>
    /// Represents base api controller with integrated IMediator service, and response mapping.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

        protected async Task<IActionResult> Send(ICommand command) 
            => MapResponse(await Mediator.Send(command));

        protected async Task<IActionResult> Send<TResult>(ICommand<TResult> command)
            => MapResponse(await Mediator.Send(command));

        protected async Task<IActionResult> Send<TResult>(IQuery<TResult> command, CancellationToken ct)
            => MapResponse(await Mediator.Send(command, ct));

        protected IActionResult MapResponse<T>(IResponse<T> response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.IsSuccess)
                return Ok(response);

            return MapError(response);
        }

        protected IActionResult MapResponse<T>(T response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            return Ok(ResponseFactory.Success(response));
        }

        protected IActionResult MapResponse(IResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.IsSuccess)
                return Ok(response);

            return MapError(response);
        }

        private IActionResult MapError(IResponse response)
        {
            if (response.Error == null)
                throw new ArgumentNullException(nameof(response.Error));

            switch (response.Error.Type)
            {
                case ErrorType.ServerError:
                    return new InternalServerErrorObjectResult(response);
                case ErrorType.NotFound:
                    return new NotFoundObjectResult(response);
                case ErrorType.InvalidData:
                    return new BadRequestObjectResult(response);
                case ErrorType.InvalidPermission:
                    return new ForbiddenObjectResult(response);
                default:
                    throw new ArgumentOutOfRangeException($"Invalid error type: {response.Error.Type}");
            }
        }
    }
}
