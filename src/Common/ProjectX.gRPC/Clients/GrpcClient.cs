using Grpc.Core;
using Microsoft.Extensions.Logging;
using ProjectX.Common;
using ProjectX.Common.Auth;
using ProjectX.Common.Exceptions;
using ProjectX.Common.JSON;
using ProjectX.Protos;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.gRPC.Clients
{
    public abstract class GrpcClient<TClient>
        where TClient : Grpc.Core.ClientBase<TClient>
    {
        readonly ITokenProvider _tokenProvider;
        readonly ILogger<GrpcClient<TClient>> _logger;
        readonly IJsonSerializer _serializer;
        readonly protected TClient Client;

        public GrpcClient(
            ITokenProvider tokenProvider,
            ILogger<GrpcClient<TClient>> logger,
            IJsonSerializer serializer,
            TClient client)
        {
            _tokenProvider = tokenProvider;
            _logger = logger;
            _serializer = serializer;
            Client = client;
        }

        protected async Task<Metadata> GetHeadersAsync(CancellationToken cancellation = default)
        {
            if (!_tokenProvider.Enabled)
                return null;

            IToken token = await _tokenProvider.GetTokenAsync(cancellation);
            if (string.IsNullOrEmpty(token.AccessToken))
            {
                throw new Exception("HttpService: TokenProvider has returned null or empty token.");
            }
            var metadata = new Metadata();
            metadata.Add("Authorization", $"Bearer {token.AccessToken}");
            return metadata;
        }

        protected async Task ManagedExecutionAsync(Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (RpcException e)
            {
                _logger.LogError(e.ToString());

                if (TryGetError(e.Trailers, out Error error))
                    HandleError(error);

                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        protected async Task<TResponse> ManagedExecutionAsync<TResponse>(Func<Task<TResponse>> func)
        {
            try
            {
                return await func();
            }
            catch (RpcException e)
            {
                _logger.LogError(e.ToString());

                if (TryGetError(e.Trailers, out Error error))
                    HandleError(error);

                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw;
            }
        }

        protected ResultOf<T> Map<T>(T data, ErrorResponse error)
          where T : class
        {
            return error != null
                ? ResultOf<T>.Failed(GrpcMapper.Map(error))
                : ResultOf<T>.Success(data);
        }

        protected Result Map(ErrorResponse error)
        {
            return error != null
                ? Result.Failed(GrpcMapper.Map(error))
                : Result.Success;
        }

        protected ResultOf<TOut> Map<TOut>(ErrorResponse error, Func<TOut> func)
         where TOut : class
        {
            return error != null
                ? ResultOf<TOut>.Failed(GrpcMapper.Map(error))
                : ResultOf<TOut>.Success(func());
        }

        private bool TryGetError(Metadata headers, out Error error)
        {
            error = null;
            bool success = false;
            try
            {
                var jsonError = headers.FirstOrDefault(t => t.Key == "error")?.Value;
                if (!string.IsNullOrEmpty(jsonError))
                    error = _serializer.Deserialize<Error>(jsonError);

                if (error != null)
                    success = true;
            }
            catch
            {
                success = false;
            }

            return success;
        }

        private void HandleError(Error error)
        {
            switch (error.Type)
            {
                case ErrorType.InvalidData:
                    throw new InvalidDataException(error);
                case ErrorType.NotFound:
                    throw new NotFoundException(error);
                case ErrorType.InvalidPermission:
                    throw new InvalidPermissionException(error);
            }
        }
    }
}
