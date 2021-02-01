using ProjectX.Common.Auth;
using System;
using System.Text;
using System.Net.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Threading;
using IdentityModel.Client;
using System.Net.Http.Headers;

namespace ProjectX.Common.Infrastructure.Auth
{
    public sealed class TokenProvider : ITokenProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TokenProviderOptions _options;

        private IToken _token;
        
        public TokenProvider(IHttpClientFactory httpClientFactory, IOptions<TokenProviderOptions> options)
        {
            _httpClientFactory = httpClientFactory;
            _options = options.Value;
        }

        #region ITokenProvider members

        public bool Enabled => _options.Enabled;

        public Task<IToken> GetTokenAsync(CancellationToken cancellationToken = default)
        {
            if (_options.Enabled && (_token == null || _token.IsExpired))
                return RequestTokenAsync(cancellationToken);
            else
                return Task.FromResult(_token);
        }

        public async Task<bool> IntrospectTokenAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            var introspectionResponse = await GetIntrospectResponseAsync(accessToken, cancellationToken);

            if (introspectionResponse.IsError)
                throw new Exception(introspectionResponse.Error);

            return introspectionResponse.IsActive;
        }

        public void Clear() => _token = null;

        #endregion

        #region Private methods

        private async Task<IToken> RequestTokenAsync(CancellationToken cancellationToken)
        {
            var tokenResponse = await GetTokenResponseAsync(cancellationToken);

            if (tokenResponse.IsError)
                throw new Exception(tokenResponse.Error);

            return _token = new Token(tokenResponse.AccessToken, tokenResponse.ExpiresIn, tokenResponse.TokenType);
        }

        private async Task<TokenResponse> GetTokenResponseAsync(CancellationToken cancellationToken)
        {
            using (var httpclient = _httpClientFactory.CreateClient("tokenClient"))
                return await httpclient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = $"{httpclient.BaseAddress}connect/token",
                    ClientId = _options.ClientId,
                    ClientSecret = _options.ClientSecret,
                    Scope = _options.Scopes,
                }, cancellationToken);
        }

        private async Task<TokenIntrospectionResponse> GetIntrospectResponseAsync(string accessToken, CancellationToken cancellationToken)
        {
            using (var httpClient = _httpClientFactory.CreateClient("tokenClient"))
            {
                var basicAuthToken = Convert.ToBase64String(Encoding.Default.GetBytes($"{_options.IdentityApiName}:{_options.IdentityApiSecret}"));
                
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthToken);

                return await httpClient.IntrospectTokenAsync(new TokenIntrospectionRequest
                {
                    Address = $"{httpClient.BaseAddress}connect/introspect",
                    ClientId = _options.ClientId,
                    ClientSecret = _options.ClientSecret,
                    Token = accessToken
                }, cancellationToken);
            }
        }        

        #endregion
    }
}
