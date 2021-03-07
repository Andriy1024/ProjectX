using System.Collections.Generic;
using System.Linq;

namespace ProjectX.Messenger.Domain
{
    public class UserConversationsView
    {
        public class Conversation
        {
            public string Id { get; set; }

            public MessageView LastMessage { get; set; }
        }

        public long UserId { get; set; }
        
        public List<Conversation> Conversations { get; set; } = new List<Conversation>();

        public void Apply(MessageCreated @event) 
        {
            Apply(new MessageView(id: @event.MessageId,
                      conversationId: @event.ConversationId,
                      authorId: @event.AuthorId,
                      content: @event.Content,
                      createdAt: @event.CreatedAt));
        }

        public void Apply(MessageView message) 
        {
            var conversation = Conversations.FirstOrDefault(c => c.Id == message.ConversationId);

            if(conversation == null) 
            {
                conversation = new Conversation() { Id = message.ConversationId };
                Conversations.Add(conversation);
            }
            
            conversation.LastMessage = message;
        }

        public void Apply(MessageUpdated @event) 
        {
            var conversation = Conversations.FirstOrDefault(c => c.LastMessage.Id == @event.MessageId);

            if(conversation != null) 
            {
                conversation.LastMessage.UpdatedAt = @event.UpdatedAt;
                conversation.LastMessage.Content = @event.Content;
            }
        }

        public void DeleteConversation(string conversationId) 
        {
            Conversations.RemoveAll(c => c.Id == conversationId);
        }
    }
}
