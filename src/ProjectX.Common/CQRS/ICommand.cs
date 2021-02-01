using MediatR;

namespace ProjectX.Common
{
    public interface ICommand : IRequest<IResponse>
    {
    }

    public interface ICommand<TResult> : IRequest<IResponse<TResult>>
    {
    }
}
