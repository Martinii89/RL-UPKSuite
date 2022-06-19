using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultStaticMeshSerializer : BaseObjectSerializer<UStaticMesh>
{
    private readonly IStreamSerializer<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializer<FkDOPBounds> _kDopBoundsSerializer;
    private readonly IStreamSerializer<FkDOPNode> _kDopNode3NewSerializer;
    private readonly IStreamSerializer<FkDOPTriangles> _kDopTrianglesSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IObjectSerializer<FStaticMeshLODModel> _staticMeshLodModel3Serializer;

    public DefaultStaticMeshSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializer<FkDOPBounds> kDopBoundsSerializer,
        IStreamSerializer<FkDOPNode> kDopNode3NewSerializer, IObjectSerializer<FStaticMeshLODModel> staticMeshLodModel3Serializer,
        IStreamSerializer<FkDOPTriangles> kDopTrianglesSerializer)
    {
        _objectSerializer = objectSerializer;
        _boxSphereBoundsSerializer = boxSphereBoundsSerializer;
        _kDopBoundsSerializer = kDopBoundsSerializer;
        _kDopNode3NewSerializer = kDopNode3NewSerializer;
        _staticMeshLodModel3Serializer = staticMeshLodModel3Serializer;
        _kDopTrianglesSerializer = kDopTrianglesSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UStaticMesh obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.FBoxSphereBounds = _boxSphereBoundsSerializer.Deserialize(objectStream.BaseStream);
        obj.BodySetup = objectStream.ReadObject() as URB_BodySetup;
        obj.FkDopBounds = _kDopBoundsSerializer.Deserialize(objectStream.BaseStream);
        obj.NewNodes = _kDopNode3NewSerializer.ReadTArrayWithElementSize(objectStream.BaseStream);
        obj.Triangles = _kDopTrianglesSerializer.ReadTArrayWithElementSize(objectStream.BaseStream);
        obj.InternalVersion = objectStream.ReadInt32();
        obj.UnkFlag = objectStream.ReadInt32();
        obj.F178ElementsCount = objectStream.ReadInt32();
        obj.F74 = objectStream.ReadInt32();
        obj.Unk = objectStream.ReadInt32();
        obj.Lods = _staticMeshLodModel3Serializer.ReadTArrayToList(objectStream);
        var unknownDataLength = (int) (obj.ExportTableItem!.SerialSize - (objectStream.BaseStream.Position - obj.ExportTableItem!.SerialOffset));
        obj.UnknownBytes = objectStream.ReadBytes(unknownDataLength);
    }

    /// <inheritdoc />
    public override void SerializeObject(UStaticMesh obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}