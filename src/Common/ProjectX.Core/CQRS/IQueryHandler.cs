using MediatR;

namespace ProjectX.Core
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, IResponse<TResponse>>
       where TQuery : IQuery<TResponse>
    {
    }
}
