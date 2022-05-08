using Core.Types;

namespace Core.Serialization.Default;

/// <inheritdoc />
public class FGuidSerializer : IStreamSerializerFor<FGuid>
{
    /// <inheritdoc />
    public FGuid Deserialize(Stream stream)
    {
        return new FGuid
        {
            A = stream.ReadUInt32(),
            B = stream.ReadUInt32(),
            C = stream.ReadUInt32(),
            D = stream.ReadUInt32()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FGuid value)
    {
        stream.WriteUInt32(value.A);
        stream.WriteUInt32(value.B);
        stream.WriteUInt32(value.C);
        stream.WriteUInt32(value.D);
    }
}