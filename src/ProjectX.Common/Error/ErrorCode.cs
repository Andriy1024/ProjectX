namespace ProjectX.Common
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
        SessionNotFound
    }
}
