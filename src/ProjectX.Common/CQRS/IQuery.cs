using MediatR;

namespace ProjectX.Common
{
    public interface IQuery<TResult> : IRequest<IResponse<TResult>>
    {
    }
}
