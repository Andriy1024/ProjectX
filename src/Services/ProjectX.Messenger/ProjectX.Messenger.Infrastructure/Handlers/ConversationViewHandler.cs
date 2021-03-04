using Marten;
using ProjectX.Core;
using ProjectX.Core.Auth;
using ProjectX.Messenger.Application;
using ProjectX.Messenger.Application.Views;
using ProjectX.Messenger.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Messenger.Infrastructure.Handlers
{
    public sealed class ConversationViewHandler : IQueryHandler<GetConversationViewQuery, ConversationView>
    {
        private readonly IDocumentSession _session;
        private readonly ICurrentUser _currentUser;

        public ConversationViewHandler(IDocumentSession session, ICurrentUser currentUser)
        {
            _session = session;
            _currentUser = currentUser;
        }

        public async Task<IResponse<ConversationView>> Handle(GetConversationViewQuery query, CancellationToken cancellationToken)
        {
            var id = new ConversationId(_currentUser.IdentityId, query.CompanionId);

            var conversation = await _session.Query<ConversationView>().FirstOrDefaultAsync(c => c.Id == id.Value, cancellationToken);

            return conversation != null
                ? ResponseFactory.Success(conversation)
                : ResponseFactory.NotFound<ConversationView>(ErrorCode.NotFound);
        }
    }
}
