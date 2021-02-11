using System;
using System.Collections.Generic;
using System.Text;
using ProjectX.Protos;

namespace ProjectX.gRPC
{

    /// <summary>
    /// Source: https://docs.microsoft.com/en-us/aspnet/core/grpc/protobuf?view=aspnetcore-3.1
    /// </summary>
    //public partial class DecimalValue
    //{
    //    private const decimal NanoFactor = 1_000_000_000;
    //    public DecimalValue(long units, int nanos)
    //    {
    //        Units = units;
    //        Nanos = nanos;
    //    }

    //    public static implicit operator decimal(DecimalValue grpcDecimal)
    //    {
    //        return grpcDecimal.Units + grpcDecimal.Nanos / NanoFactor;
    //    }

    //    // Example: 12345.6789 -> { units = 12345, nanos = 678900000 }
    //    public static implicit operator DecimalValue(decimal value)
    //    {
    //        var units = decimal.ToInt64(value);
    //        var nanos = decimal.ToInt32((value - units) * NanoFactor);
    //        return new DecimalValue(units, nanos);
    //    }
    //}
}
