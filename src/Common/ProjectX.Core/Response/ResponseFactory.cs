namespace ProjectX.Core
{
    public static class ResponseFactory
    {
        public static IResponse Success() => Response.Success;
        public static IResponse<T> Success<T>(T data) => new Response<T>(data);
        public static IPaginatedResponse<T> Success<T>(T data, int total) => new PaginationResponse<T>(data, total);

        public static IResponse Failed(IError error) => new Response(error);
        public static IResponse<T> Failed<T>(IError error) => new Response<T>(error);
        public static IPaginatedResponse<T> PaginationFailed<T>(IError error) => new PaginationResponse<T>(error);

        public static IResponse ServerError(ErrorCode code, string message = null) => new Response(Error.ServerError(code, message));
        public static IResponse ServerError(string message) => new Response(Error.ServerError(message));

        public static IResponse<T> ServerError<T>(ErrorCode code, string message = null) => new Response<T>(Error.ServerError(code, message));
        public static IResponse<T> ServerError<T>(string message) => new Response<T>(Error.ServerError(message));

        public static IResponse NotFound(ErrorCode code, string message = null) => new Response(Error.NotFound(code, message));
        public static IResponse<T> NotFound<T>(ErrorCode code, string message = null) => new Response<T>(Error.NotFound(code, message));

        public static IResponse InvalidData(ErrorCode code, string message = null) => new Response(Error.InvalidData(code, message));
        public static IResponse<T> InvalidData<T>(ErrorCode code, string message = null) => new Response<T>(Error.InvalidData(code, message));

        public static IResponse InvalidPermission(ErrorCode code, string message = null) => new Response(Error.InvalidPermission(code, message));
        public static IResponse<T> InvalidPermission<T>(ErrorCode code, string message = null) => new Response<T>(Error.InvalidPermission(code, message));

        public static IResponse<T> Map<T>(ResultOf<T> resultOf)
            where T : class
            => resultOf.IsFailed
                ? new Response<T>(resultOf.Error)
                : new Response<T>(resultOf.Result);
        
        public static IResponse Map(Result result)
            => result.IsFailed
                ? new Response(result.Error)
                : Response.Success;
    }
}
