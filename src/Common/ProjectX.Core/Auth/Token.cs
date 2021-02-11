using System;

namespace ProjectX.Core.Auth
{
    public sealed class Token : IToken
    {
        public string AccessToken { get; }

        public DateTime ExpirationDate { get; }

        public string Type { get; }

        public bool IsExpired => ExpirationDate <= DateTime.UtcNow;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="accessToken">Access token string value</param>
        /// <param name="expiresIn">Token filetime in seconds</param>
        /// <param name="type">Token type</param>
        public Token(string accessToken, int expiresIn, string type)
        {
            AccessToken = accessToken;
            ExpirationDate = DateTime.UtcNow.AddSeconds(expiresIn - 60);
            Type = type;
        }
    }
}
