using Marten;
using ProjectX.Core;
using ProjectX.Core.Auth;
using ProjectX.Messenger.Application;
using ProjectX.Messenger.Domain;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Messenger.Infrastructure.Handlers
{
    public sealed class ConversationViewHandler 
        : IQueryHandler<ConversationViewQuery, ConversationView>,
          IQueryHandler<UserConversationsQuery, List<UserConversationsView.Conversation>>
    {
        private readonly IDocumentSession _session;
        private readonly ICurrentUser _currentUser;

        public ConversationViewHandler(IDocumentSession session, ICurrentUser currentUser)
        {
            _session = session;
            _currentUser = currentUser;
        }

        public async Task<IResponse<ConversationView>> Handle(ConversationViewQuery query, CancellationToken cancellationToken)
        {
            var id = new ConversationId(_currentUser.IdentityId, query.UserId);

            var conversation = await _session.Query<ConversationView>().FirstOrDefaultAsync(c => c.Id == id.Value, cancellationToken);

            return conversation != null
                ? ResponseFactory.Success(conversation)
                : ResponseFactory.NotFound<ConversationView>(ErrorCode.NotFound);
        }

        public async Task<IResponse<List<UserConversationsView.Conversation>>> Handle(UserConversationsQuery query, CancellationToken cancellationToken)
        {
            var conversations = await _session.Query<UserConversationsView>().FirstOrDefaultAsync(c => c.UserId == _currentUser.IdentityId, cancellationToken);

            return conversations != null
                ? ResponseFactory.Success(conversations.Conversations)
                : ResponseFactory.NotFound<List<UserConversationsView.Conversation>>(ErrorCode.NotFound);
        }
    }
}
