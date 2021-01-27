using System;

namespace ProjectX.Common
{
    public class Response<T> : Response, IResponse<T>
    {
        public T Data { get; set; }

        /// <summary>
        /// For deserialization
        /// </summary>
        public Response() { }

        public Response(IError error)
            : base(error)
        {
        }

        public Response(T data)
            : base()
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            Data = data;
        }
    }
}
