﻿using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultStaticMeshLodModelSerializer : BaseObjectSerializer<FStaticMeshLODModel>
{
    private readonly IStreamSerializer<FByteBulkData> _bulkDataSerializer;
    private readonly IStreamSerializer<ColorStream> _colorStreamSerializer;
    private readonly IObjectSerializer<FStaticMeshSection> _staticMeshSectionSerializer;
    private readonly IStreamSerializer<UvStream> _uvStreamSerializer;
    private readonly IStreamSerializer<VertexStream> _vertexStreamSerializer;

    public DefaultStaticMeshLodModelSerializer(IStreamSerializer<FByteBulkData> bulkDataSerializer,
        IObjectSerializer<FStaticMeshSection> staticMeshSectionSerializer, IStreamSerializer<VertexStream> vertexStreamSerializer,
        IStreamSerializer<UvStream> uvStreamSerializer, IStreamSerializer<ColorStream> colorStreamSerializer)
    {
        _bulkDataSerializer = bulkDataSerializer;
        _staticMeshSectionSerializer = staticMeshSectionSerializer;
        _vertexStreamSerializer = vertexStreamSerializer;
        _uvStreamSerializer = uvStreamSerializer;
        _colorStreamSerializer = colorStreamSerializer;
    }


    /// <inheritdoc />
    public override void DeserializeObject(FStaticMeshLODModel obj, IUnrealPackageStream objectStream)
    {
        obj.FBulkData = _bulkDataSerializer.Deserialize(objectStream.BaseStream);
        obj.FStaticMeshSections = _staticMeshSectionSerializer.ReadTArrayToList(objectStream);
        obj.VertexStream = _vertexStreamSerializer.Deserialize(objectStream.BaseStream);
        obj.UvStream = _uvStreamSerializer.Deserialize(objectStream.BaseStream);
        obj.ColorStream = _colorStreamSerializer.Deserialize(objectStream.BaseStream);
        obj.NumVerts = objectStream.ReadInt32();
        obj.Indicies = objectStream.BulkReadTArray(stream => stream.ReadUInt16());
        obj.Indicies2 = objectStream.BulkReadTArray(stream => stream.ReadUInt16());
        obj.Indicies3 = objectStream.BulkReadTArray(stream => stream.ReadUInt16());
    }

    /// <inheritdoc />
    public override void SerializeObject(FStaticMeshLODModel obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}