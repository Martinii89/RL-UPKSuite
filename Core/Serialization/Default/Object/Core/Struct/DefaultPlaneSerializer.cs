using Core.Classes.Core.Structs;

namespace Core.Serialization.Default.Object.Core.Struct;

public class DefaultPlaneSerializer : IStreamSerializerFor<FPlane>
{
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;

    public DefaultPlaneSerializer(IStreamSerializerFor<FVector> vectorSerializer)
    {
        _vectorSerializer = vectorSerializer;
    }

    /// <inheritdoc />
    public FPlane Deserialize(Stream stream)
    {
        return new FPlane
        {
            xyz = _vectorSerializer.Deserialize(stream),
            w = stream.ReadSingle()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FPlane value)
    {
        _vectorSerializer.Serialize(stream, value.xyz);
        stream.Write(value.w);
    }
}