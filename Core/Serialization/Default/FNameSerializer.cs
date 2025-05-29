using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;

namespace RlUpk.Core.Serialization.Default;

/// <summary>
///     Serializer for FNames
/// </summary>
public class FNameSerializer : IStreamSerializer<FName>
{
    /// <inheritdoc />
    public FName Deserialize(Stream stream)
    {
        return new FName
        {
            NameIndex = stream.ReadInt32(),
            InstanceNumber = stream.ReadInt32()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FName value)
    {
        stream.WriteInt32(value.NameIndex);
        stream.WriteInt32(value.InstanceNumber);
    }
}