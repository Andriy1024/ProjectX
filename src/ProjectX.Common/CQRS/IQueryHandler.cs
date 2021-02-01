using MediatR;

namespace ProjectX.Common
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, IResponse<TResponse>>
       where TQuery : IQuery<TResponse>
    {
    }
}
