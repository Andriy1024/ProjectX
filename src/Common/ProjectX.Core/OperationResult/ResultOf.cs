namespace ProjectX.Core
{
    public struct ResultOf<T>
        where T : class
    {
        public ResultStatus Status { get; private set; }

        public IError Error { get; private set; }

        public T Result { get; private set; }

        public bool IsFailed => Status == ResultStatus.Failed;

        public ResultOf(ResultStatus status, IError error, T result)
        {
            if (status == ResultStatus.Failed)
                Utill.ThrowIfNull(error, nameof(error));
            else
                Utill.ThrowIfNull(result, nameof(result));

            Status = status;
            Error = error;
            Result = result;
        }

        public static ResultOf<T> Success(T result) 
            => new ResultOf<T>(ResultStatus.Success, error: null, result: result);

        public static ResultOf<T> Failed(IError error) 
            => new ResultOf<T>(ResultStatus.Failed, error: error, result: null);

        public static implicit operator ResultOf<T>(Error error) => Failed(error);

        public static implicit operator ResultOf<T>(T value) => Success(value);

        public override string ToString()
            => Status == ResultStatus.Success
                ? "Result is succeeded."
                : $"Result is failed. {Error.ToString()}";
    }
}
