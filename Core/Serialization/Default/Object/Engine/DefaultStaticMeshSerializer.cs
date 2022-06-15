using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultStaticMeshSerializer : BaseObjectSerializer<UStaticMesh>
{
    private readonly IStreamSerializer<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializer<FkDOPBounds> _kDopBoundsSerializer;
    private readonly IStreamSerializer<FkDOPNode3New> _kDopNode3NewSerializer;
    private readonly IStreamSerializer<FkDOPTriangles> _kDOPTrianglesSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializer<FStaticMeshLODModel3> _staticMeshLodModel3Serializer;

    public DefaultStaticMeshSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer, IStreamSerializer<FkDOPBounds> kDopBoundsSerializer,
        IStreamSerializer<FkDOPNode3New> kDopNode3NewSerializer, IStreamSerializer<FStaticMeshLODModel3> staticMeshLodModel3Serializer,
        IStreamSerializer<FkDOPTriangles> kDopTrianglesSerializer)
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
    public override void DeserializeObject(UStaticMesh obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.FBoxSphereBounds = _boxSphereBoundsSerializer.Deserialize(objectStream.BaseStream);
        obj.BodySetup = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream)) as URB_BodySetup;
        obj.FkDopBounds = _kDopBoundsSerializer.Deserialize(objectStream.BaseStream);
        obj.NewNodes = _kDopNode3NewSerializer.ReadTArrayWithElementSize(objectStream.BaseStream);
        obj.Triangles = _kDOPTrianglesSerializer.ReadTArrayWithElementSize(objectStream.BaseStream);
        obj.InternalVersion = objectStream.ReadInt32();
        obj.UnkFlag = objectStream.ReadInt32();
        obj.F178ElementsCount = objectStream.ReadInt32();
        obj.F74 = objectStream.ReadInt32();
        obj.Unk = objectStream.ReadInt32();
        var lodCount = objectStream.ReadInt32();
        objectStream.BaseStream.Move(-4);
        if (lodCount > 1)
        {
            Debugger.Break();
        }

        obj.Lods = _staticMeshLodModel3Serializer.ReadTArrayToList(objectStream.BaseStream);

        //obj.LODInfo = objectStream.ReadTarray(stream =>
        //{
        //    var res = new FStaticMeshLODInfo();
        //    res.Elements = stream.ReadTarray(stream1 =>
        //    {
        //        var element = new FStaticMeshLODElement();
        //        element.Material = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UMaterialInterface;
        //        element.bEnableShadowCasting = stream1.ReadInt32() == 1;
        //        element.bEnableCollision = stream1.ReadInt32() == 1;
        //        return element;
        //    });
        //    return res;
        //});

        var unknownDataLength = (int) (obj.ExportTableItem!.SerialSize - (objectStream.BaseStream.Position - obj.ExportTableItem!.SerialOffset));
        obj.UnknownBytes = objectStream.ReadBytes(unknownDataLength);
    }

    /// <inheritdoc />
    public override void SerializeObject(UStaticMesh obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}