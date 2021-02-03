namespace ProjectX.Common.Auth
{
    public interface ICurrentUser
    {
        long IdentityId { get; }
        string IdentityRole { get; }
    }
}
