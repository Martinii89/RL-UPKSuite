using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultNavigationMeshBaseSerializer : BaseObjectSerializer<UNavigationMeshBase>
{
    private readonly IObjectSerializer<FBorderEdgeInfo> _borderEdgeInfoSerializer;
    private readonly IStreamSerializer<FBox> _boxSerializer;
    private readonly IObjectSerializer<FEdgeStorageDatum> _edgeStorageDatumSerializer;
    private readonly IStreamSerializer<FMatrix> _matrixSerializer;
    private readonly IObjectSerializer<FMeshVertex> _meshVertexSerializer;
    private readonly IObjectSerializer<FNavMeshEdgeBase> _navMeshEdgeBaseSerializer;
    private readonly IObjectSerializer<FNavMeshPolyBase> _navMeshPolyBaseSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultNavigationMeshBaseSerializer(IObjectSerializer<UObject> objectSerializer, IObjectSerializer<FMeshVertex> meshVertexSerializer,
        IObjectSerializer<FEdgeStorageDatum> edgeStorageDatumSerializer, IObjectSerializer<FNavMeshPolyBase> navMeshPolyBaseSerializer,
        IStreamSerializer<FMatrix> matrixSerializer, IObjectSerializer<FBorderEdgeInfo> borderEdgeInfoSerializer,
        IObjectSerializer<FNavMeshEdgeBase> navMeshEdgeBaseSerializer, IStreamSerializer<FBox> boxSerializer)
    {
        _objectSerializer = objectSerializer;
        _meshVertexSerializer = meshVertexSerializer;
        _edgeStorageDatumSerializer = edgeStorageDatumSerializer;
        _navMeshPolyBaseSerializer = navMeshPolyBaseSerializer;
        _matrixSerializer = matrixSerializer;
        _borderEdgeInfoSerializer = borderEdgeInfoSerializer;
        _navMeshEdgeBaseSerializer = navMeshEdgeBaseSerializer;
        _boxSerializer = boxSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UNavigationMeshBase obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        obj.NavMeshVersionNum = objectStream.ReadInt32();
        obj.VersionAtGenerationTime = objectStream.ReadInt32();
        obj.Verts = _meshVertexSerializer.ReadTArrayToList(objectStream);
        obj.EdgeStorageData = _edgeStorageDatumSerializer.ReadTArrayToList(objectStream);
        if (obj.EdgeStorageData.Any(x => x.ClassName != "FNavMeshEdgeBase"))
        {
            throw new NotImplementedException();
        }

        obj.Polys = _navMeshPolyBaseSerializer.ReadTArrayToList(objectStream);
        obj.LocalToWorld = _matrixSerializer.Deserialize(objectStream.BaseStream);
        obj.WorldToLocal = _matrixSerializer.Deserialize(objectStream.BaseStream);
        if (obj.Outer is APylon)
        {
            obj.BorderEdgeSegments = _borderEdgeInfoSerializer.ReadTArrayToList(objectStream);
        }

        obj.BoxBounds = _boxSerializer.Deserialize(objectStream.BaseStream);

        var remaining = obj.ExportTableItem.SerialOffset + obj.ExportTableItem.SerialSize - objectStream.BaseStream.Position;
        if (remaining % 38 != 0 && obj.EdgeStorageData.Count * 38 != remaining)
        {
            Debugger.Break();
        }

        obj.Edges = _navMeshEdgeBaseSerializer.ReadTArrayToList(objectStream, obj.EdgeStorageData.Count);
    }

    /// <inheritdoc />
    public override void SerializeObject(UNavigationMeshBase obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteInt32(obj.NavMeshVersionNum);
        objectStream.WriteInt32(obj.VersionAtGenerationTime);
        objectStream.WriteTArray(obj.Verts, _meshVertexSerializer);
        objectStream.WriteTArray(obj.EdgeStorageData, _edgeStorageDatumSerializer);
        objectStream.WriteTArray(obj.Polys, _navMeshPolyBaseSerializer);
        _matrixSerializer.Serialize(objectStream.BaseStream, obj.LocalToWorld);
        _matrixSerializer.Serialize(objectStream.BaseStream, obj.WorldToLocal);
        if (obj.Outer is APylon)
        {
            objectStream.WriteTArray(obj.BorderEdgeSegments, _borderEdgeInfoSerializer);
        }

        _boxSerializer.Serialize(objectStream.BaseStream, obj.BoxBounds);
        foreach (var edge in obj.Edges)
        {
            _navMeshEdgeBaseSerializer.SerializeObject(edge, objectStream);
        }
    }
}

public class DefaultMeshVertexSerializer : BaseObjectSerializer<FMeshVertex>
{
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultMeshVertexSerializer(IStreamSerializer<FVector> vectorSerializer)
    {
        _vectorSerializer = vectorSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(FMeshVertex obj, IUnrealPackageStream objectStream)
    {
        obj.vector = _vectorSerializer.Deserialize(objectStream.BaseStream);
        obj.PolyIndices = objectStream.ReadTArray(stream => stream.ReadUInt16());
    }

    /// <inheritdoc />
    public override void SerializeObject(FMeshVertex obj, IUnrealPackageStream objectStream)
    {
        _vectorSerializer.Serialize(objectStream.BaseStream, obj.vector);
        objectStream.WriteTArray(obj.PolyIndices, (stream, arg2) => stream.WriteUInt16(arg2));
    }
}

public class DefaultEdgeStorageDatumSerializer : BaseObjectSerializer<FEdgeStorageDatum>
{
    /// <inheritdoc />
    public override void DeserializeObject(FEdgeStorageDatum obj, IUnrealPackageStream objectStream)
    {
        obj.DataPtrOffset = objectStream.ReadInt32();
        obj.DataSize = objectStream.ReadInt16();
        obj.ClassName = objectStream.ReadFNameStr();
    }

    /// <inheritdoc />
    public override void SerializeObject(FEdgeStorageDatum obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteInt32(obj.DataPtrOffset);
        objectStream.WriteInt16(obj.DataSize);
        objectStream.WriteFName(obj.ClassName);
    }
}

public class DefaultNavMeshPolyBaseSerializer : BaseObjectSerializer<FNavMeshPolyBase>
{
    private readonly IStreamSerializer<FBox> _boxSerializer;
    private readonly IObjectSerializer<FCoverReference> _coverReferenceSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultNavMeshPolyBaseSerializer(IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FBox> boxSerializer,
        IObjectSerializer<FCoverReference> coverReferenceSerializer)
    {
        _vectorSerializer = vectorSerializer;
        _boxSerializer = boxSerializer;
        _coverReferenceSerializer = coverReferenceSerializer;
    }

    public override void DeserializeObject(FNavMeshPolyBase obj, IUnrealPackageStream objectStream)
    {
        obj.PolyVerts = objectStream.ReadTArray(stream => stream.ReadUInt16());
        obj.PolyEdges = objectStream.ReadTArray(stream => stream.ReadUInt16());
        obj.PolyCenter = _vectorSerializer.Deserialize(objectStream.BaseStream);
        obj.PolyNormal = _vectorSerializer.Deserialize(objectStream.BaseStream);
        obj.BoxBounds = _boxSerializer.Deserialize(objectStream.BaseStream);
        obj.PolyCover = _coverReferenceSerializer.ReadTArrayToList(objectStream);
        obj.PolyHeight = objectStream.ReadSingle();
    }

    public override void SerializeObject(FNavMeshPolyBase obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteTArray(obj.PolyVerts, (stream, arg2) => stream.WriteUInt16(arg2));
        objectStream.WriteTArray(obj.PolyEdges, (stream, arg2) => stream.WriteUInt16(arg2));
        _vectorSerializer.Serialize(objectStream.BaseStream, obj.PolyCenter);
        _vectorSerializer.Serialize(objectStream.BaseStream, obj.PolyNormal);
        _boxSerializer.Serialize(objectStream.BaseStream, obj.BoxBounds);
        objectStream.WriteTArray(obj.PolyCover, _coverReferenceSerializer);
        objectStream.WriteSingle(obj.PolyHeight);
    }
}

public class DefaultFCoverReferenceSerializer : BaseObjectSerializer<FCoverReference>
{
    private readonly IStreamSerializer<FGuid> _guidSerializer;

    public DefaultFCoverReferenceSerializer(IStreamSerializer<FGuid> guidSerializer)
    {
        _guidSerializer = guidSerializer;
    }

    public override void DeserializeObject(FCoverReference obj, IUnrealPackageStream objectStream)
    {
        var maybeActor = objectStream.ReadObject();
        obj.Actor = maybeActor as AActor;
        obj.Guid = _guidSerializer.Deserialize(objectStream.BaseStream);
        obj.SlotIDx = objectStream.ReadInt32();
    }

    public override void SerializeObject(FCoverReference obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}

public class DefaultBorderEdgeInfoSerializer : BaseObjectSerializer<FBorderEdgeInfo>
{
    public override void DeserializeObject(FBorderEdgeInfo obj, IUnrealPackageStream objectStream)
    {
        obj.Vert0 = objectStream.ReadUInt16();
        obj.Vert1 = objectStream.ReadUInt16();
        obj.Poly = objectStream.ReadUInt16();
    }

    public override void SerializeObject(FBorderEdgeInfo obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteUInt16(obj.Vert0);
        objectStream.WriteUInt16(obj.Vert1);
        objectStream.WriteUInt16(obj.Poly);
    }
}

public class DefaultNavMeshEdgeBaseSerializer : BaseObjectSerializer<FNavMeshEdgeBase>
{
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultNavMeshEdgeBaseSerializer(IStreamSerializer<FVector> vectorSerializer)
    {
        _vectorSerializer = vectorSerializer;
    }

    public override void DeserializeObject(FNavMeshEdgeBase obj, IUnrealPackageStream objectStream)
    {
        obj.data = objectStream.ReadBytes(38);
        //obj.Vert0 = objectStream.ReadUInt16();
        //obj.Vert1 = objectStream.ReadUInt16();
        //obj.Poly0 = objectStream.ReadUInt16();
        //obj.Poly1 = objectStream.ReadUInt16();
        //obj.EdgeLEngth = objectStream.ReadSingle();
        //obj.EffectiveEdgeLength = objectStream.ReadSingle();
        //obj.EdgeCenter = _vectorSerializer.Deserialize(objectStream.BaseStream);
        //obj.EdgeType = objectStream.ReadByte();
    }

    public override void SerializeObject(FNavMeshEdgeBase obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteBytes(obj.data);
    }
}