using ProjectX.Core;
using ProjectX.Protos;
using System;

namespace ProjectX.gRPC
{
    internal class GrpcMapper
    {
        public static ErrorResponse Map(IError error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            return new ErrorResponse()
            {
                Message = error.Message,
                Code = (int)error.ErrorCode,
                Type = Map(error.Type)
            };
        }

        public static IError Map(ErrorResponse error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            return new Error(type: Map(error.Type), errorCode: (ErrorCode)error.Code, error.Message);
        }

        public static ErrorType Map(GRPCErrorType type)
        {
            switch (type)
            {
                case GRPCErrorType.ServerError:
                    return ErrorType.ServerError;
                case GRPCErrorType.NotFound:
                    return ErrorType.NotFound;
                case GRPCErrorType.InvalidData:
                    return ErrorType.InvalidData;
                case GRPCErrorType.InvalidPermission:
                    return ErrorType.InvalidPermission;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid error type: {type}");
            }
        }

        public static GRPCErrorType Map(ErrorType type)
        {
            switch (type)
            {
                case ErrorType.ServerError:
                    return GRPCErrorType.ServerError;
                case ErrorType.NotFound:
                    return GRPCErrorType.NotFound;
                case ErrorType.InvalidData:
                    return GRPCErrorType.InvalidData;
                case ErrorType.InvalidPermission:
                    return GRPCErrorType.InvalidPermission;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid error type: {type}");
            }
        }

        #region Storage

        //public static FileType Map(GRPCFileType fileType)
        //{
        //    int value = (int)fileType;
        //    if (Enum.IsDefined(typeof(FileType), value))
        //    {
        //        return (FileType)value;
        //    }
        //    else
        //        throw new ArgumentOutOfRangeException($"Invalid file type: {fileType}");
        //}

        //public static GRPCFileType Map(FileType fileType)
        //{
        //    int value = (int)fileType;
        //    if (Enum.IsDefined(typeof(GRPCFileType), value))
        //    {
        //        return (GRPCFileType)value;
        //    }
        //    else
        //        throw new ArgumentOutOfRangeException($"Invalid file type: {fileType}");
        //}

        private static TOut MapEnum<TOut, TIn>(TIn source)
            where TOut : struct
            where TIn : struct
        {
            var value = source.ToString();

            if (Enum.TryParse<TOut>(value, out var result))
            {
                return result;
            }
            else
                throw new ArgumentOutOfRangeException($"Enum mapping failed: source - {source.GetType().Name} value - {source}, destination - {typeof(TOut).Name}");
        }

        #endregion
    }
}
