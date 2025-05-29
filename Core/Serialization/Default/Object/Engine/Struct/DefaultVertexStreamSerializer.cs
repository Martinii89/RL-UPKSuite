using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;

namespace RlUpk.Core.Serialization.Default.Object.Engine.Struct;

public class DefaultVertexStreamSerializer : IStreamSerializer<VertexStream>
{
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultVertexStreamSerializer(IStreamSerializer<FVector> vectorSerializer)
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
        stream.WriteInt32(value.VertexSize);
        stream.WriteInt32(value.VertexCount);

        stream.WriteInt32(value.VertexStreamArray.ElementSize);
        _vectorSerializer.WriteTArray(stream, value.VertexStreamArray.ToArray());
    }
}

public class DefaultkDOPTrianglesSerializer : IStreamSerializer<FkDOPTriangles>
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
        stream.WriteInt16s(value.Triangles);
    }
}