using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProjectX.Infrastructure.REST
{
    public class TooManyRequestsObjectResult : ObjectResult
    {
        public TooManyRequestsObjectResult(object error) :
            base(error)
        {
            StatusCode = StatusCodes.Status429TooManyRequests;
        }
    }
}
