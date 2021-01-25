namespace ProjectX.Common
{
    public readonly struct Result
    {
        public ResultStatus Status { get; }

        public IError Error { get; }

        public bool IsFailed => Status == ResultStatus.Failed;

        public Result(ResultStatus status, IError error)
        {
            if (status == ResultStatus.Failed)
                Utill.ThrowIfNull(error, nameof(error));

            Status = status;
            Error = error;
        }

        public static readonly Result Success = new Result(ResultStatus.Success, null);

        public static Result Failed(IError error) 
            => new Result(ResultStatus.Failed, error);

        public override string ToString()
            => Status == ResultStatus.Success
                ? "OperationResult succeeded."
                : $"OperationResult failed. {Error.ToString()}";
        
        public static implicit operator Result(Error error) => Failed(error);
    }
}
