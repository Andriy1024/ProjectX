using ProjectX.Core.Extensions;

namespace ProjectX.Core
{
    public class Error : IError
    {
        public Error(ErrorType type, ErrorCode errorCode, string message)
        {
            Message = message ?? errorCode.GetDescription();
            Type = type;
            ErrorCode = errorCode;
        }

        public string Message { get; }
        public ErrorType Type { get; }
        public ErrorCode ErrorCode { get; }

        public static Error ServerError(ErrorCode code, string message = null)
            => new Error(ErrorType.ServerError, code, message);

        public static Error ServerError(string message = null)
            => new Error(ErrorType.ServerError, ErrorCode.ServerError, message);

        public static Error NotFound(ErrorCode code, string message = null)
            => new Error(ErrorType.NotFound, code, message);

        public static Error InvalidData(ErrorCode code, string message = null)
            => new Error(ErrorType.InvalidData, code, message);

        public static Error InvalidPermission(ErrorCode code, string message = null)
            => new Error(ErrorType.InvalidPermission, code, message);

        public override string ToString()
            => $"Error: Type - {Type}, Code - {ErrorCode}, Message - {Message}.";
    }
}
