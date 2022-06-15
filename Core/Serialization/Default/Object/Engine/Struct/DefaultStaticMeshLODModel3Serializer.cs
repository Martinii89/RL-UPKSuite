using Core.Classes.Engine.Structs;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultStaticMeshLodModel3Serializer : IStreamSerializer<FStaticMeshLODModel3>
{
    private readonly IStreamSerializer<FByteBulkData> _bulkDataSerializer;
    private readonly IStreamSerializer<ColorStream> _colorStreamSerializer;
    private readonly IStreamSerializer<FStaticMeshSection> _staticMeshSectionSerializer;
    private readonly IStreamSerializer<UvStream> _uvStreamSerializer;
    private readonly IStreamSerializer<VertexStream> _vertexStreamSerializer;

    public DefaultStaticMeshLodModel3Serializer(IStreamSerializer<FByteBulkData> bulkDataSerializer,
        IStreamSerializer<FStaticMeshSection> staticMeshSectionSerializer, IStreamSerializer<VertexStream> vertexStreamSerializer,
        IStreamSerializer<UvStream> uvStreamSerializer, IStreamSerializer<ColorStream> colorStreamSerializer)
    {
        _bulkDataSerializer = bulkDataSerializer;
        _staticMeshSectionSerializer = staticMeshSectionSerializer;
        _vertexStreamSerializer = vertexStreamSerializer;
        _uvStreamSerializer = uvStreamSerializer;
        _colorStreamSerializer = colorStreamSerializer;
    }

    /// <inheritdoc />
    public FStaticMeshLODModel3 Deserialize(Stream stream)
    {
        var model3 = new FStaticMeshLODModel3();
        model3.FBulkData = _bulkDataSerializer.Deserialize(stream);
        model3.FStaticMeshSections = _staticMeshSectionSerializer.ReadTArrayToList(stream);
        model3.VertexStream = _vertexStreamSerializer.Deserialize(stream);
        model3.UvStream = _uvStreamSerializer.Deserialize(stream);
        model3.ColorStream = _colorStreamSerializer.Deserialize(stream);
        model3.NumVerts = stream.ReadInt32();
        model3.Indicies = stream.ReadTarrayWithElementSize(stream1 => stream1.ReadUInt16());
        model3.Indicies2 = stream.ReadTarrayWithElementSize(stream1 => stream1.ReadUInt16());
        model3.Indicies3 = stream.ReadTarrayWithElementSize(stream1 => stream1.ReadUInt16());
        return model3;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FStaticMeshLODModel3 value)
    {
        throw new NotImplementedException();
    }
}