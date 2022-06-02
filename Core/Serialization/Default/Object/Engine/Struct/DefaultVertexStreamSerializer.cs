using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultVertexStreamSerializer : IStreamSerializerFor<VertexStream>
{
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;

    public DefaultVertexStreamSerializer(IStreamSerializerFor<FVector> vectorSerializer)
    {
        _vectorSerializer = vectorSerializer;
    }

    /// <inheritdoc />
    public VertexStream Deserialize(Stream stream)
    {
        return new VertexStream
        {
            VertexSize = stream.ReadInt32(),
            VertexCount = stream.ReadInt32(),
            VertexStreamArray = _vectorSerializer.ReadTArrayWithElementSize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, VertexStream value)
    {
        throw new NotImplementedException();
    }
}

public class DefaultkDOPTrianglesSerializer : IStreamSerializerFor<FkDOPTriangles>
{
    /// <inheritdoc />
    public FkDOPTriangles Deserialize(Stream stream)
    {
        return new FkDOPTriangles
        {
            Triangles = stream.ReadInt16s(4)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FkDOPTriangles value)
    {
        throw new NotImplementedException();
    }
}