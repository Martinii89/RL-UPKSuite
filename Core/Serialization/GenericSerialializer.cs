using Core.Types;
using Syroot.BinaryData;

namespace Core.Serialization;

/// <summary>
///     Interface used by <see cref="TArray{T}" /> deserialization to deserialize native and generic elements implementing
///     the <see cref="IBinaryDeserializableClass" /> interface.
/// </summary>
public static class GenericSerializer
{
    /// <summary>
    ///     Deserialize a value or a generic object implementing the <see cref="IBinaryDeserializableClass" /> interface.
    /// </summary>
    /// <param name="o"></param>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException">Thrown for unsupported element types</exception>
    public static object Deserialize(object o, BinaryReader reader)
    {
        switch (o)
        {
            case IBinaryDeserializableClass serializable:
                serializable.Deserialize(reader);
                return o;
            case int:
                return reader.ReadInt32();
            case ushort:
                return reader.ReadUInt16();
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Writes a element to the stream
    /// </summary>
    /// <typeparam name="T">The type to write</typeparam>
    /// <param name="elem">The value to write</param>
    /// <param name="stream">The stream to write to</param>
    /// <exception cref="NotImplementedException">Thrown for unsupported types</exception>
    public static void Serialize<T>(T elem, Stream stream) where T : new()
    {
        switch (elem)
        {
            case IBinarySerializableClass serializable:
                serializable.Serialize(stream);
                break;
            case int i:
                stream.WriteInt32(i);
                break;
            case ushort s:
                stream.WriteUInt16(s);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}