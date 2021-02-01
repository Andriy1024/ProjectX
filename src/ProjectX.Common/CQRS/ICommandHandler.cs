using MediatR;

namespace ProjectX.Common
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, IResponse>
      where TCommand : ICommand
    {
    }

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, IResponse<TResponse>>
        where TCommand : ICommand<TResponse>
    {
    }
}
