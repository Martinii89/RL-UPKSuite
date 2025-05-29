using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Core.Struct;

public class DefaultBoxSerializer : IStreamSerializer<FBox>
{
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultBoxSerializer(IStreamSerializer<FVector> vectorSerializer)
    {
        _vectorSerializer = vectorSerializer;
    }

    /// <inheritdoc />
    public FBox Deserialize(Stream stream)
    {
        return new FBox
        {
            Min = _vectorSerializer.Deserialize(stream),
            Max = _vectorSerializer.Deserialize(stream),
            IsValid = (byte) stream.ReadByte()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FBox value)
    {
        _vectorSerializer.Serialize(stream, value.Min);
        _vectorSerializer.Serialize(stream, value.Max);
        stream.Write(value.IsValid);
    }
}