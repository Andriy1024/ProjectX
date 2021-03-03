using ProjectX.Core;
using ProjectX.Core.Auth;
using ProjectX.Messenger.Application;
using ProjectX.Messenger.Domain;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Messenger.Infrastructure.Handlers
{
    public sealed class ConversationCommandHandler 
        : ICommandHandler<SendMessage>,
          ICommandHandler<DeleteMessage>,
          ICommandHandler<UpdateMessage>
    {
        private readonly IEventStore _eventStore;
        private readonly ICurrentUser _currentUser;

        public ConversationCommandHandler(IEventStore eventStore, ICurrentUser currentUser)
        {
            _eventStore = eventStore;
            _currentUser = currentUser;
        }

        public async Task<IResponse> Handle(SendMessage command, CancellationToken cancellationToken)
        {
            var id = new ConversationId(_currentUser.IdentityId, command.Recipient);
            var conversation = await _eventStore.LoadAsync<Conversation>(id);
            conversation ??= Conversation.Start(_currentUser.IdentityId, command.Recipient);
            conversation.AddMessage(messageId: Guid.NewGuid(), _currentUser.IdentityId, command.Content);
            await _eventStore.StoreAsync(conversation);
            return ResponseFactory.Success();
        }

        public async Task<IResponse> Handle(DeleteMessage command, CancellationToken cancellationToken)
        {
            var conversation = await _eventStore.LoadAsync<Conversation>(command.ConversationId);
            if(conversation == null) return ResponseFactory.NotFound(ErrorCode.NotFound);
            conversation.DeleteMessage(command.MessageId);
            await _eventStore.StoreAsync(conversation);
            return ResponseFactory.Success();
        }

        public async Task<IResponse> Handle(UpdateMessage command, CancellationToken cancellationToken)
        {
            var conversation = await _eventStore.LoadAsync<Conversation>(command.ConversationId);
            if (conversation == null) return ResponseFactory.NotFound(ErrorCode.NotFound);
            conversation.UpdateMessage(command.MessageId, command.Content);
            await _eventStore.StoreAsync(conversation);
            return ResponseFactory.Success();
        }
    }
}
