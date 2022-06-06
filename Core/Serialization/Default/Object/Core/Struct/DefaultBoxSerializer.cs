using Core.Classes.Core.Structs;

namespace Core.Serialization.Default.Object.Core.Struct;

public class DefaultBoxSerializer : IStreamSerializerFor<FBox>
{
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;

    public DefaultBoxSerializer(IStreamSerializerFor<FVector> vectorSerializer)
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