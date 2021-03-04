namespace ProjectX.Core
{
    public enum ErrorCode
    {
        //General
        ServerError,
        InvalidData,
        InvalidPermission,
        NotFound,
        InboxMessageAlreadyHandled,

        //Identity
        NoIdentityIdInAccessToken,
        NoIdentityRoleInAccessToken,
        SessionNotFound,
        SessionInBlackList,
        EmailAlreadyConfirmed,

        EmailNotAvailable,
        UserNotFound,

        //Blog
        ArticleNotFound,
        CommentNotFound,
        AuthorNotFound,

        //Messenger
        ConversationMessageNotFound,
        ConversationNotFound,
    }
}
