using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Common.Auth
{
    /// <summary>
    /// Uses for handling logic with access token
    /// </summary>
    public interface ITokenProvider
    {
        /// <summary>
        /// Gets instance of IToken.
        /// </summary>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation, 
        /// containing inctance of IToken.
        /// </returns>
        Task<IToken> GetTokenAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates instance of IToken.
        /// </summary>
        /// <param name="accessToken">Access token for validation.</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation, 
        /// containing Boolean value that indicates if IToken is valid.
        /// </returns>
        Task<bool> IntrospectTokenAsync(string accessToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Determines is internal token validation enabled or not
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Removes current access token
        /// </summary>
        void Clear();
    }
}
