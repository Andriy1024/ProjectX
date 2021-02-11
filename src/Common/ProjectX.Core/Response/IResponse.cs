namespace ProjectX.Core
{
    public interface IResponse
    {
        bool IsSuccess { get; }
        IError Error { get; }
    }

    public interface IResponse<T> : IResponse
    {
        T Data { get; }
    }

    public interface IPaginatedResponse<T> : IResponse<T>
    {
        int Total { get; }
    }
}
