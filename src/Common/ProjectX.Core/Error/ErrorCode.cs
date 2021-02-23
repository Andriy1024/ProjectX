namespace ProjectX.Core
{
    public enum ErrorCode
    {
        //General
        ServerError,
        InvalidData,
        InvalidPermission,
        NotFound,

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

        MessageAlreadyHandled
    }
}
