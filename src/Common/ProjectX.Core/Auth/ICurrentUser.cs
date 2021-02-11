namespace ProjectX.Core.Auth
{
    public interface ICurrentUser
    {
        long IdentityId { get; }
        string IdentityRole { get; }
    }
}
