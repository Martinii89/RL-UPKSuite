using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultkDOPBoundsSerializer : IStreamSerializer<FkDOPBounds>
{
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultkDOPBoundsSerializer(IStreamSerializer<FVector> vectorSerializer)
    {
        _vectorSerializer = vectorSerializer;
    }


    /// <inheritdoc />
    public FkDOPBounds Deserialize(Stream stream)
    {
        return new FkDOPBounds
        {
            V1 = _vectorSerializer.Deserialize(stream),
            V2 = _vectorSerializer.Deserialize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FkDOPBounds value)
    {
        _vectorSerializer.Serialize(stream, value.V1);
        _vectorSerializer.Serialize(stream, value.V2);
    }
}