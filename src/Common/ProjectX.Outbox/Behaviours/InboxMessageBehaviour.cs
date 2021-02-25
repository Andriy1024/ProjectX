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
        private readonly IOutboxTransaction _outboxManager;

        public InboxMessageBehaviour(IOutboxTransaction outboxManager)
        {
            _outboxManager = outboxManager;
        }

        public async Task<TResponse> Handle(TRequest integrationEvent, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (await _outboxManager.HasInboxAsync(integrationEvent.Id)) 
            {
                throw new InvalidDataException(ErrorCode.MessageAlreadyHandled);
            }

            await _outboxManager.AddInboxAsync(integrationEvent);

            return await next();
        }
    }
}
