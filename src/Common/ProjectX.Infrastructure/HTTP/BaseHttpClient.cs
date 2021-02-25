using IdentityModel.Client;
using ProjectX.Core;
using ProjectX.Core.Auth;
using ProjectX.Core.Exceptions;
using ProjectX.Core.JSON;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Infrastructure.HTTP
{
    public abstract class BaseHttpClient
    {
        #region Fields

        private readonly HttpClient _httpClient;
        private readonly ITokenProvider _tokenProvider;
        private readonly IJsonSerializer _jsonSerializer;

        #endregion

        #region Constructors

        protected BaseHttpClient(HttpClient httpClient, ITokenProvider tokenProvider, IJsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _tokenProvider = tokenProvider;
            _jsonSerializer = jsonSerializer;
        }

        #endregion

        #region Protected methods for derivied http clients

        /// <summary>
        /// Sends GET request to specified service
        /// </summary>
        /// <typeparam name="TOut">Type of data should be returned</typeparam>
        /// <param name="path">Request uri</param>
        /// <param name="cancellationToken">Instance of CancellationToken</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation, 
        /// containing inctance of TOut type.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="JsonSerializationException"></exception>
        /// <exception cref="Exception">Internal server error</exception>
        protected async Task<TOut> GetAsync<TOut>(string path, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync();
            using HttpResponseMessage response = await _httpClient.GetAsync(path, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
            return await DeserializeResponse<TOut>(response);
        }


        /// <summary>
        /// Sends POST request to specified service
        /// </summary>
        /// <typeparam name="TIn">Type of send object</typeparam>
        /// <typeparam name="TOut">Type of data should be returned</typeparam>
        /// <param name="path">Request uri</param>
        /// <param name="message">Request content</param>
        /// <param name="cancellationToken">Instance of CancellationToken</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation, 
        /// containing inctance of TOut type.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="JsonSerializationException"></exception>
        /// <exception cref="Exception">Internal server error</exception>
        protected async Task<TOut> PostAsync<TIn, TOut>(string path, TIn message, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync();
            var content = SerializeContent(message);
            using HttpResponseMessage response = await _httpClient.PostAsync(path, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
            return await DeserializeResponse<TOut>(response);
        }

        /// <summary>
        /// Sends POST request to specified service with multiple form data content
        /// </summary>
        /// <typeparam name="TOut">Type of data should be returned</typeparam>
        /// <param name="path">Request uri</param>
        /// <param name="formData">Request content</param>
        /// <param name="cancellationToken">Instance of CancellationToken</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation, 
        /// containing instance of TOut type.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception">Internal server error</exception>
        protected async Task<TOut> PostAsync<TOut>(string path, MultipartFormDataContent formData, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync(cancellationToken);
            using HttpResponseMessage response = await _httpClient.PostAsync(path, formData, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response, cancellationToken);
            return await DeserializeResponse<TOut>(response);
        }

        /// <summary>
        /// Sends POST request to specified service
        /// </summary>
        /// <typeparam name="TIn">Type of send object</typeparam>
        /// <param name="path">Request uri</param>
        /// <param name="message">Request content</param>
        /// <param name="cancellationToken">Instance of CancellationToken</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception">Internal server error</exception>
        protected async Task PostAsync<TIn>(string path, TIn message, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync();
            var content = SerializeContent(message);
            using HttpResponseMessage response = await _httpClient.PostAsync(path, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
        }

        protected async Task PostAsync(string path, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync();
            using HttpResponseMessage response = await _httpClient.PostAsync(path, null, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
        }

        /// <summary>
        /// Sends PUT request to specified service
        /// </summary>
        /// <typeparam name="TIn">Type of send object</typeparam>
        /// <typeparam name="TOut">Type of data should be returned</typeparam>
        /// <param name="path">Request uri</param>
        /// <param name="message">Request content</param>
        /// <param name="cancellationToken">Instance of CancellationToken</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation, 
        /// containing inctance of TOut type.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="JsonSerializationException"></exception>
        /// <exception cref="Exception">Internal server error</exception>
        protected async Task<TOut> PutAsync<TIn, TOut>(string path, TIn message, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync();
            var content = SerializeContent(message);
            using HttpResponseMessage response = await _httpClient.PutAsync(path, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
            return await DeserializeResponse<TOut>(response);
        }

        /// <summary>
        /// Sends PUT request to specified service
        /// </summary>
        /// <typeparam name="TIn">Type of send object</typeparam>
        /// <param name="path">Request uri</param>
        /// <param name="message">Request content</param>
        /// <param name="cancellationToken">Instance of CancellationToken</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception">Internal server error</exception>
        protected async Task PutAsync<TIn>(string path, TIn message, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync();
            var content = SerializeContent(message);
            using HttpResponseMessage response = await _httpClient.PutAsync(path, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
        }

        /// <summary>
        /// Sends DELETE request to specified service
        /// </summary>
        /// <typeparam name="TOut">Type of data should be returned</typeparam>
        /// <param name="path">Request uri</param>
        /// <param name="message">Request content</param>
        /// <param name="cancellationToken">Instance of CancellationToken</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation, 
        /// containing inctance of TOut type.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="JsonSerializationException"></exception>
        /// <exception cref="Exception">Internal server error</exception>
        protected async Task<TOut> DeleteAsync<TOut>(string path, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync();
            using HttpResponseMessage response = await _httpClient.DeleteAsync(path, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
            return await DeserializeResponse<TOut>(response);
        }

        /// <summary>
        /// Sends DELETE request to specified service
        /// </summary>s
        /// <param name="path">Request uri</param>
        /// <param name="cancellationToken">Instance of CancellationToken</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception">Internal server error</exception>
        protected async Task DeleteAsync(string path, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync();
            using HttpResponseMessage response = await _httpClient.DeleteAsync(path, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
        }

        /// <summary>
        /// Sends DELETE request to specified service
        /// </summary>s
        /// <param name="path">Request uri</param>
        /// <param name="cancellationToken">Instance of CancellationToken</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception">Internal server error</exception>
        protected async Task DeleteAsync<TIn>(string path, TIn body, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                Content = SerializeContent(body),
                RequestUri = new Uri(_httpClient.BaseAddress, path)
            };

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
        }

        /// <summary>
        /// Sends DELETE request to specified service
        /// </summary>s
        /// <param name="path">Request uri</param>
        /// <param name="cancellationToken">Instance of CancellationToken</param>
        /// <returns>
        /// The System.Threading.Tasks.Task that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="HttpRequestException"></exception>
        /// <exception cref="Exception">Internal server error</exception>
        protected async Task<TOut> DeleteAsync<TIn, TOut>(string path, TIn body, CancellationToken cancellationToken = default)
        {
            await SetTokenAsync();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                Content = SerializeContent(body),
                RequestUri = new Uri(_httpClient.BaseAddress, path)
            };

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);
            return await DeserializeResponse<TOut>(response);
        }

        #endregion

        #region Private 
        private async Task SetTokenAsync(CancellationToken cancellationToken = default)
        {
            if (!_tokenProvider.Enabled)
                return;

            IToken token = await _tokenProvider.GetTokenAsync(cancellationToken);
            if (string.IsNullOrEmpty(token.AccessToken))
            {
                throw new Exception("HttpService: TokenProvider has returned null or empty token.");
            }
            _httpClient.SetBearerToken(token.AccessToken);
        }

        private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response, CancellationToken cancellationToken = default)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await DeserializeResponseAsync<Error>(response);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        throw new InvalidDataException(error);
                    case HttpStatusCode.NotFound:
                        throw new NotFoundException(error);
                    case HttpStatusCode.Forbidden:
                        throw new InvalidPermissionException(error);
                    default:
                        throw new Exception($"Internal http error. Request url: {response.RequestMessage.RequestUri}. Response: {error.Message} {response.StatusCode}.");
                }
            }
        }

        private async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            var result = default(T);
            var isDeserialized = false;
            
            try
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                result = _jsonSerializer.Deserialize<T>(stringContent);
                isDeserialized = true;
            }
            catch (Exception)
            {
                isDeserialized = false;
            }

            if (!isDeserialized || result == null)
            {
                var stringContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Internal http error. Request url: {response.RequestMessage.RequestUri}. Response: {stringContent}, code: {response.StatusCode}.");
            }

            return result;
        }

        private Task<TOut> DeserializeResponse<TOut>(HttpResponseMessage response)
        {
            return DeserializeResponseAsync<TOut>(response);
        }

        private StringContent SerializeContent<TIn>(TIn obj)
        {
            string json = _jsonSerializer.Serialize(obj, obj.GetType());
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            return content;
        }

        #endregion
    }
}
