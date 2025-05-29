using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Core.Struct;

public class DefaultFColorStreamSerializer : IStreamSerializer<FColor>
{
    /// <inheritdoc />
    public FColor Deserialize(Stream stream)
    {
        return new FColor
        {
            R = (byte) stream.ReadByte(),
            G = (byte) stream.ReadByte(),
            B = (byte) stream.ReadByte(),
            A = (byte) stream.ReadByte()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FColor value)
    {
        stream.WriteByte(value.R);
        stream.WriteByte(value.G);
        stream.WriteByte(value.B);
        stream.WriteByte(value.A);
    }
}