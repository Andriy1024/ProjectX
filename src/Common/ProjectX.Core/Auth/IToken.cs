using System;

namespace ProjectX.Core.Auth
{
    /// <summary>
    /// Represents data about jwt access token
    /// </summary>
    public interface IToken
    {
        /// <summary>
        /// Access token string value
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Access token expiration time
        /// </summary>
        DateTime ExpirationDate { get; }

        /// <summary>
        /// Token type
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Determines if token is expired
        /// </summary>
        bool IsExpired { get; }
    }
}
