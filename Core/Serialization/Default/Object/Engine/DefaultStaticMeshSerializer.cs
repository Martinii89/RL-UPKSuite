using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultStaticMeshSerializer : BaseObjectSerializer<UStaticMesh>
{
    private readonly IStreamSerializerFor<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializerFor<FkDOPBounds> _kDopBoundsSerializer;
    private readonly IStreamSerializerFor<FkDOPNode3New> _kDopNode3NewSerializer;
    private readonly IStreamSerializerFor<FkDOPTriangles> _kDOPTrianglesSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializerFor<FStaticMeshLODModel3> _staticMeshLodModel3Serializer;

    public DefaultStaticMeshSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializerFor<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer, IStreamSerializerFor<FkDOPBounds> kDopBoundsSerializer,
        IStreamSerializerFor<FkDOPNode3New> kDopNode3NewSerializer, IStreamSerializerFor<FStaticMeshLODModel3> staticMeshLodModel3Serializer,
        IStreamSerializerFor<FkDOPTriangles> kDopTrianglesSerializer)
    {
        _objectSerializer = objectSerializer;
        _boxSphereBoundsSerializer = boxSphereBoundsSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _kDopBoundsSerializer = kDopBoundsSerializer;
        _kDopNode3NewSerializer = kDopNode3NewSerializer;
        _staticMeshLodModel3Serializer = staticMeshLodModel3Serializer;
        _kDOPTrianglesSerializer = kDopTrianglesSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UStaticMesh obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.FBoxSphereBounds = _boxSphereBoundsSerializer.Deserialize(objectStream);
        obj.BodySetup = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as URB_BodySetup;
        obj.FkDopBounds = _kDopBoundsSerializer.Deserialize(objectStream);
        obj.NewNodes = _kDopNode3NewSerializer.ReadTArrayWithElementSize(objectStream);
        obj.Triangles = _kDOPTrianglesSerializer.ReadTArrayWithElementSize(objectStream);
        obj.InternalVersion = objectStream.ReadInt32();
        obj.UnkFlag = objectStream.ReadInt32();
        obj.F178ElementsCount = objectStream.ReadInt32();
        obj.F74 = objectStream.ReadInt32();
        obj.Unk = objectStream.ReadInt32();
        var lodCount = objectStream.ReadInt32();
        objectStream.Move(-4);
        if (lodCount > 1)
        {
            Debugger.Break();
        }

        obj.Lods = _staticMeshLodModel3Serializer.ReadTArrayToList(objectStream);

        var unknownDataLength = (int) (obj.ExportTableItem!.SerialSize - (objectStream.Position - obj.ExportTableItem!.SerialOffset));
        obj.UnknownBytes = objectStream.ReadBytes(unknownDataLength);
    }

    /// <inheritdoc />
    public override void SerializeObject(UStaticMesh obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}