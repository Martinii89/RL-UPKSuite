using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultBoxSphereBoundsSerializer : IStreamSerializer<FBoxSphereBounds>
{
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultBoxSphereBoundsSerializer(IStreamSerializer<FVector> vectorSerializer)
    {
        _vectorSerializer = vectorSerializer;
    }


    /// <inheritdoc />
    public FBoxSphereBounds Deserialize(Stream stream)
    {
        return new FBoxSphereBounds
        {
            Origin = _vectorSerializer.Deserialize(stream),
            BoxExtent = _vectorSerializer.Deserialize(stream),
            SphereRadius = stream.ReadSingle()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FBoxSphereBounds value)
    {
        throw new NotImplementedException();
    }
}