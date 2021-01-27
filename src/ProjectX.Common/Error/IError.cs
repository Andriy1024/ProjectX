namespace ProjectX.Common
{
    public interface IError
    {
        string Message { get; }
        ErrorType Type { get; }
        ErrorCode ErrorCode { get; }
    }
}
