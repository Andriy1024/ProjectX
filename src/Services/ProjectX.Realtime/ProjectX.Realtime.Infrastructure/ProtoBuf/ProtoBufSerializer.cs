using System;
using System.Buffers;
using System.IO;
//using ProtoSerializer = ProtoBuf.Serializer;

namespace ProjectX.Realtime.Infrastructure.ProtoBuf
{
    /// <summary>
    /// Provides simple wrapper for <a href="https://github.com/protobuf-net/protobuf-net">protobuf-net</a> serializer.
    /// </summary>
    //public static class ProtoBufSerializer
    //{
    //    public static T Deserialize<T>(ref ReadOnlySequence<byte> buffer) => ProtoSerializer.Deserialize<T>(buffer);

    //    public static T Deserialize<T>(ReadOnlyMemory<byte> buffer) => ProtoSerializer.Deserialize<T>(buffer);

    //    public static object Deserialize(Type targetType, byte[] buffer) => ProtoSerializer.Deserialize(targetType, new MemoryStream(buffer));

    //    public static ReadOnlyMemory<byte> Serialize(object data)
    //    {
    //        ArrayBufferWriter<byte> writer = new ArrayBufferWriter<byte>();
    //        ProtoSerializer.Serialize(writer, data);
    //        return writer.WrittenMemory;
    //    }
    //}
}
