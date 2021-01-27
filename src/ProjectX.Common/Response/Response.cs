namespace ProjectX.Common
{
    public class Response : IResponse
    {
        public static readonly Response Success = new Response();

        public static readonly System.Threading.Tasks.Task<Response> Task = System.Threading.Tasks.Task.FromResult(Success);

        public bool IsSuccess { get; private set; }

        public IError Error { get; private set; }

        public Response()
        {
            IsSuccess = true;
        }

        public Response(IError error = null)
        {
            Utill.ThrowIfNull(error, nameof(error));

            IsSuccess = false;
            Error = error;
        }
    }
}
