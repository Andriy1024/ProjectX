using MediatR;

namespace ProjectX.Core
{
    public interface IHasTransaction { }

    public interface ICommand : IRequest<IResponse>, IHasTransaction
    {
    }

    public interface ICommand<TResult> : IRequest<IResponse<TResult>>, IHasTransaction
    {
    }
}
