namespace ProjectX.Core
{
    public class PaginationResponse<T> : Response<T>, IPaginatedResponse<T>
    {
        public int Total { get; }

        public PaginationResponse(T data, int total)
            : base(data)
        {
            Total = total;
        }

        public PaginationResponse(IError error)
            : base(error) { }
    }
}
