using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using ProjectX.Common;
using ProjectX.Common.Exceptions;
using ProjectX.Common.JSON;
using System;
using System.Threading.Tasks;

namespace ProjectX.gRPC
{
    public class ErrorHandlingInterceptor : Interceptor
    {
        readonly IJsonSerializer _serializer;
        readonly ILogger<ErrorHandlingInterceptor> _logger;

        public ErrorHandlingInterceptor(IJsonSerializer serializer, ILogger<ErrorHandlingInterceptor> logger)
        {
            _serializer = serializer;
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (NotFoundException e)
            {
                _logger.LogError(e.ToString());
                throw new RpcException(new Status(StatusCode.NotFound, e.Message, e), GetMetadata(e.Error));
            }
            catch (InvalidPermissionException e)
            {
                _logger.LogError(e.ToString());
                throw new RpcException(new Status(StatusCode.PermissionDenied, e.Message, e), GetMetadata(e.Error));
            }
            catch (InvalidDataException e)
            {
                _logger.LogError(e.ToString());
                throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message, e), GetMetadata(e.Error));
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
                throw new RpcException(new Status(StatusCode.Unknown, e.Message, e));
            }
        }

        private Metadata GetMetadata(IError error)
        {
            var jsonError = _serializer.Serialize(error);
            var metadata = new Metadata();
            metadata.Add("error", jsonError);
            return metadata;
        }
    }
}
