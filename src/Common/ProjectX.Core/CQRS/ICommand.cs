using MediatR;

namespace ProjectX.Core
{
    public interface ICommand : IRequest<IResponse>
    {
    }

    public interface ICommand<TResult> : IRequest<IResponse<TResult>>
    {
    }
}
