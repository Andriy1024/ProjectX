using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Infrastructure.REST;
using ProjectX.Core;
using System;

namespace ProjectX.Infrastructure.Controllers
{
    /// <summary>
    /// Represents base api controller with integrated IMediator service, and response mapping.
    /// </summary>
    [ApiControllerAttribute]
    [Produces("application/json")]
    public abstract class ApiController : ControllerBase
    {
        protected IMediator Mediator => HttpContext.RequestServices.GetRequiredService<IMediator>();

        protected IActionResult MapResponse<T>(IResponse<T> response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            if (response.IsSuccess)
                return Ok(response);

            return MapError(response);
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
