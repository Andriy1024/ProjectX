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

        EmailNotAvailable,
        UserNotFound
    }
}
