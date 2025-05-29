using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Serialization.Default;

/// <summary>
///     Reads a object index as a int32 value
/// </summary>
public class ObjectIndexSerializer : IStreamSerializer<ObjectIndex>
{
    /// <inheritdoc />
    public ObjectIndex Deserialize(Stream stream)
    {
        return new ObjectIndex(stream.ReadInt32());
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, ObjectIndex value)
    {
        stream.WriteInt32(value.Index);
    }
}