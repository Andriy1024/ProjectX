using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Core
{
    public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public abstract Task<IResponse<TResponse>> Handle(TQuery request, CancellationToken cancellationToken);

        protected virtual IResponse<TResponse> ErrorResponse(IError error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            return ResponseFactory.Failed<TResponse>(error);
        }

        protected virtual IResponse<TResponse> SuccessResponse(TResponse result)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));

            return ResponseFactory.Success(result);
        }
    }
}
