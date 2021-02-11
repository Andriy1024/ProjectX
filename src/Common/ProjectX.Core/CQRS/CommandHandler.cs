using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Core
{
    public abstract class CommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public abstract Task<IResponse<TResponse>> Handle(TCommand request, CancellationToken cancellationToken);

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

    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public abstract Task<IResponse> Handle(TCommand request, CancellationToken cancellationToken);

        protected virtual IResponse ErrorResponse(IError error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            return ResponseFactory.Failed(error);
        }

        protected virtual IResponse SuccessResponse()
        {
            return ResponseFactory.Success();
        }
    }
}
