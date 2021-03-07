using MediatR;
using ProjectX.Core;
using ProjectX.Core.Exceptions;
using ProjectX.Core.IntegrationEvents;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Outbox
{
    public sealed class InboxMessageBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IIntegrationEvent
    {
        private readonly IOutboxTransactionContext _outboxManager;

        public InboxMessageBehaviour(IOutboxTransactionContext outboxManager)
        {
            _outboxManager = outboxManager;
        }

        public async Task<TResponse> Handle(TRequest integrationEvent, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (await _outboxManager.HasInboxAsync(integrationEvent.Id)) 
            {
                throw new InvalidDataException(ErrorCode.InboxMessageAlreadyHandled);
            }

            await _outboxManager.AddInboxAsync(integrationEvent);

            return await next();
        }
    }
}
