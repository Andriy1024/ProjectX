using System;

namespace ProjectX.Core.Exceptions
{
    public abstract class BaseException : Exception
    {
        public IError Error { get; }
        public BaseException(IError error)
            => Error = error;
        public BaseException(ErrorType type, ErrorCode code, string message = null)
            => Error = new Error(type, code, message);
    }

    public class InvalidDataException : BaseException
    {
        public InvalidDataException() : this(ErrorCode.InvalidData) { }
        public InvalidDataException(IError error) : base(error) {}
        public InvalidDataException(ErrorCode code, string message = null) 
            : base(ErrorType.InvalidData, code, message) {}
    }

    public class InvalidPermissionException : BaseException
    {
        public InvalidPermissionException() : this(ErrorCode.InvalidPermission) { }
        public InvalidPermissionException(IError error) : base(error) {}
        public InvalidPermissionException(ErrorCode code, string message = null) 
            : base(ErrorType.InvalidPermission, code, message) {}
    }

    public class NotFoundException : BaseException
    {
        public NotFoundException() : this(ErrorCode.NotFound) { }
        public NotFoundException(IError error) : base(error) {}
        public NotFoundException(ErrorCode code, string message = null) 
            : base(ErrorType.NotFound, code, message){}
    }
}
