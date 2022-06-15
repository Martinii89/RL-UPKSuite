using Core.Classes.Core.Structs;

namespace Core.Serialization.Default.Object.Core.Struct;

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