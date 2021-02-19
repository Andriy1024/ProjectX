using MediatR;
using ProjectX.Core;
using ProjectX.Core.Exceptions;
using ProjectX.Core.IntegrationEvents;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.MessageBus.Outbox
{
    //public class InboxMessageBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    //    where TRequest : IIntegrationEvent
    //{
    //    private readonly IOutboxManager _outboxManager;

    //    public InboxMessageBehaviour(IOutboxManager outboxManager)
    //    {
    //        _outboxManager = outboxManager;
    //    }

    //    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    //    {
    //        if (await _outboxManager.HasInboxAsync(request.Id)) 
    //        {
    //            throw new InvalidDataException(ErrorCode.MessageAlreadyHandled);
    //        }

    //        await _outboxManager.AddInboxAsync(request);

    //        return await next();
    //    }
    //}
}
