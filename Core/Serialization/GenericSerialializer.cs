using Core.Types;

namespace Core.Serialization;

/// <summary>
/// Interface used by <see cref="TArray{T}"/> deserialization to deserialize native and generic elements implementing the <see cref="IBinaryDeserializableClass"/> interface.
/// </summary>
public static class GenericSerializer
{
    /// <summary>
    /// Deserialize a value or a generic object implementing the <see cref="IBinaryDeserializableClass"/> interface.
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
}