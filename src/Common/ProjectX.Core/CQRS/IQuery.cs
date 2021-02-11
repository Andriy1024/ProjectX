using MediatR;

namespace ProjectX.Core
{
    public interface IQuery<TResult> : IRequest<IResponse<TResult>>
    {
    }
}
