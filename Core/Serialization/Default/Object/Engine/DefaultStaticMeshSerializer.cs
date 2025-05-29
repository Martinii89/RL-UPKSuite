using System.Diagnostics;

using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultStaticMeshSerializer : BaseObjectSerializer<UStaticMesh>
{
    private readonly IStreamSerializer<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FkDOPBounds> _kDopBoundsSerializer;
    private readonly IStreamSerializer<FkDOPNode> _kDopNode3NewSerializer;
    private readonly IStreamSerializer<FkDOPTriangles> _kDopTrianglesSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializer<FRotator> _rotatorSerializer;
    private readonly IObjectSerializer<FStaticMeshLODModel> _staticMeshLodModel3Serializer;

    public DefaultStaticMeshSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializer<FkDOPBounds> kDopBoundsSerializer,
        IStreamSerializer<FkDOPNode> kDopNode3NewSerializer, IObjectSerializer<FStaticMeshLODModel> staticMeshLodModel3Serializer,
        IStreamSerializer<FkDOPTriangles> kDopTrianglesSerializer, IStreamSerializer<FRotator> rotatorSerializer, IStreamSerializer<FGuid> guidSerializer)
    {
        _objectSerializer = objectSerializer;
        _boxSphereBoundsSerializer = boxSphereBoundsSerializer;
        _kDopBoundsSerializer = kDopBoundsSerializer;
        _kDopNode3NewSerializer = kDopNode3NewSerializer;
        _staticMeshLodModel3Serializer = staticMeshLodModel3Serializer;
        _kDopTrianglesSerializer = kDopTrianglesSerializer;
        _rotatorSerializer = rotatorSerializer;
        _guidSerializer = guidSerializer;
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
        if (obj.UnkFlag != 0)
        {
            Debugger.Break();
        }

        obj.F178ElementsCount = objectStream.ReadInt32();
        if (obj.F178ElementsCount != 0)
        {
            Debugger.Break();
        }

        obj.F74 = objectStream.ReadInt32();
        obj.Unk = objectStream.ReadInt32();
        obj.LODModels = _staticMeshLodModel3Serializer.ReadTArrayToList(objectStream);
        //Only the count is serialized. not the elements data
        obj.LodInfoCount = objectStream.ReadInt32();
        obj.ThumbnailAngle = _rotatorSerializer.Deserialize(objectStream.BaseStream);
        obj.ThumbnailDistance = objectStream.ReadSingle();
        obj.HighResSourceMeshName = objectStream.ReadFString();
        obj.HighResSourceMeshCRC = objectStream.ReadUInt32();
        obj.LightingGuid = _guidSerializer.Deserialize(objectStream.BaseStream);
        obj.Unk2 = objectStream.ReadInt32();
        obj.UnkIntArray = objectStream.ReadTArray(stream => stream.ReadInt32());
        obj.Unk3 = objectStream.ReadInt32();
        obj.Unk4 = objectStream.ReadInt32();
        obj.Unk5 = objectStream.ReadInt32();

        var unknownDataLength = (int) (obj.ExportTableItem!.SerialSize - (objectStream.BaseStream.Position - obj.ExportTableItem!.SerialOffset));
        if (unknownDataLength > 0)
        {
            Debugger.Break();
            obj.UnknownBytes = objectStream.ReadBytes(unknownDataLength);
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UStaticMesh obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        _boxSphereBoundsSerializer.Serialize(objectStream.BaseStream, obj.FBoxSphereBounds);
        objectStream.WriteObject(obj.BodySetup);
        _kDopBoundsSerializer.Serialize(objectStream.BaseStream, obj.FkDopBounds);
        objectStream.BulkWriteTArray(obj.NewNodes, _kDopNode3NewSerializer);
        objectStream.BulkWriteTArray(obj.Triangles, _kDopTrianglesSerializer);
        objectStream.WriteInt32(obj.InternalVersion);
        objectStream.WriteInt32(obj.UnkFlag);
        objectStream.WriteInt32(obj.F178ElementsCount);
        objectStream.WriteInt32(obj.F74);
        objectStream.WriteInt32(obj.Unk);
        objectStream.WriteTArray(obj.LODModels, (stream, model) => _staticMeshLodModel3Serializer.SerializeObject(model, stream));
        objectStream.WriteInt32(obj.LodInfoCount);
        _rotatorSerializer.Serialize(objectStream.BaseStream, obj.ThumbnailAngle);
        objectStream.WriteSingle(obj.ThumbnailDistance);
        objectStream.WriteFString(obj.HighResSourceMeshName);
        objectStream.WriteUInt32(obj.HighResSourceMeshCRC);
        _guidSerializer.Serialize(objectStream.BaseStream, obj.LightingGuid);
        objectStream.WriteInt32(obj.Unk2);
        objectStream.WriteTArray(obj.UnkIntArray, (stream, i) => stream.WriteInt32(i));
        objectStream.WriteInt32(obj.Unk3);
        objectStream.WriteInt32(obj.Unk4);
        objectStream.WriteInt32(obj.Unk5);
    }
}