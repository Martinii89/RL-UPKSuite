using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultkDOPBoundsSerializer : IStreamSerializerFor<FkDOPBounds>
{
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;

    public DefaultkDOPBoundsSerializer(IStreamSerializerFor<FVector> vectorSerializer)
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

    public void Serialize(Stream stream, FkDOPBounds value)
    {
        throw new NotImplementedException();
    }
}