using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Core.Struct;

public class DefaultRotatorSerializer : IStreamSerializer<FRotator>
{
    /// <inheritdoc />
    public FRotator Deserialize(Stream stream)
    {
        return new FRotator
        {
            Pitch = stream.ReadInt32(),
            Yaw = stream.ReadInt32(),
            Roll = stream.ReadInt32()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FRotator value)
    {
        stream.Write(value.Pitch);
        stream.Write(value.Yaw);
        stream.Write(value.Roll);
    }
}