using ProjectX.Core.SeedWork;

namespace ProjectX.Infrastructure.Auth
{
    public sealed class TokenProviderOptions : IOptions
    {
        public bool Enabled { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Scopes { get; set; }

        public string IdentityApiName { get; set; }

        public string IdentityApiSecret { get; set; }
    }
}
