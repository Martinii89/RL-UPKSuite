using System.Diagnostics;

using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultModelSerializer : BaseObjectSerializer<UModel>
{
    private readonly IStreamSerializer<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializer<FBspNode> _bspNodeSerializer;
    private readonly IObjectSerializer<FBspSurf> _bspSurfSerializer;
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FLightmassPrimitiveSettings> _lightmassPrimitiveSettingsSerializer;
    private readonly IStreamSerializer<FModelVertex> _modelVertexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;
    private readonly IStreamSerializer<FVert> _vertSerializer;

    public DefaultModelSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FBspNode> bspNodeSerializer,
        IObjectSerializer<FBspSurf> bspSurfSerializer, IStreamSerializer<FVert> vertSerializer,
        IStreamSerializer<FGuid> guidSerializer, IStreamSerializer<FLightmassPrimitiveSettings> lightmassPrimitiveSettingsSerializer,
        IStreamSerializer<FModelVertex> modelVertexSerializer)
    {
        _objectSerializer = objectSerializer;
        _boxSphereBoundsSerializer = boxSphereBoundsSerializer;
        _vectorSerializer = vectorSerializer;
        _bspNodeSerializer = bspNodeSerializer;
        _bspSurfSerializer = bspSurfSerializer;
        _vertSerializer = vertSerializer;
        _guidSerializer = guidSerializer;
        _lightmassPrimitiveSettingsSerializer = lightmassPrimitiveSettingsSerializer;
        _modelVertexSerializer = modelVertexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UModel obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.Bounds = _boxSphereBoundsSerializer.Deserialize(objectStream.BaseStream);
        obj.Vectors = objectStream.BulkReadTArray(stream => _vectorSerializer.Deserialize(stream.BaseStream));
        obj.Points = objectStream.BulkReadTArray(stream => _vectorSerializer.Deserialize(stream.BaseStream));
        obj.Nodes = objectStream.BulkReadTArray(stream => _bspNodeSerializer.Deserialize(stream.BaseStream));

        obj.Surfs.Super = objectStream.ReadObject();
        obj.Surfs.Data = _bspSurfSerializer.ReadTArrayToList(objectStream);
        obj.Verts = _vertSerializer.ReadTArrayWithElementSize(objectStream.BaseStream);
        obj.NumSharedSides = objectStream.ReadInt32();
        obj.NumZones = objectStream.ReadInt32();
        if (obj.NumZones > 0)
        {
            Debugger.Break();
        }

        obj.Polys = objectStream.ReadObject();
        obj.LeafHulls = objectStream.BulkReadTArray(stream => stream.ReadInt32());
        obj.Leaves = objectStream.BulkReadTArray(stream => stream.ReadInt32());
        obj.RootOutside = objectStream.ReadUInt32();
        obj.Linked = objectStream.ReadUInt32();
        obj.PortalNodes = objectStream.BulkReadTArray(stream => stream.ReadInt32());
        obj.NumVertices = objectStream.ReadInt32();
        obj.VertexBuffer.Vertices = _modelVertexSerializer.ReadTArrayWithElementSize(objectStream.BaseStream);
        obj.lightingGuid = _guidSerializer.Deserialize(objectStream.BaseStream);
        obj.LightmassPrimitiveSettings = _lightmassPrimitiveSettingsSerializer.ReadTArrayToList(objectStream.BaseStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UModel obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        _boxSphereBoundsSerializer.Serialize(objectStream.BaseStream, obj.Bounds);
        objectStream.BulkWriteTArray(obj.Vectors, _vectorSerializer);
        objectStream.BulkWriteTArray(obj.Points, _vectorSerializer);
        objectStream.BulkWriteTArray(obj.Nodes, _bspNodeSerializer);
        objectStream.WriteObject(obj.Surfs.Super);
        objectStream.WriteTArray(obj.Surfs.Data, _bspSurfSerializer);
        objectStream.BulkWriteTArray(obj.Verts, _vertSerializer);
        objectStream.WriteInt32(obj.NumSharedSides);
        objectStream.WriteInt32(obj.NumZones);
        objectStream.WriteObject(obj.Polys);
        objectStream.BulkWriteTArray(obj.PortalNodes, (stream, i) => stream.WriteInt32(i));
        objectStream.BulkWriteTArray(obj.Leaves, (stream, i) => stream.WriteInt32(i));
        objectStream.WriteUInt32(obj.RootOutside);
        objectStream.WriteUInt32(obj.Linked);
        objectStream.BulkWriteTArray(obj.PortalNodes, (stream, i) => stream.WriteInt32(i));
        objectStream.WriteInt32(obj.NumVertices);
        objectStream.BulkWriteTArray(obj.VertexBuffer.Vertices, _modelVertexSerializer);
        _guidSerializer.Serialize(objectStream.BaseStream, obj.lightingGuid);
        objectStream.WriteTArray(obj.LightmassPrimitiveSettings, _lightmassPrimitiveSettingsSerializer);
    }
}

public class DefaultBspNodeSerializer : IStreamSerializer<FBspNode>
{
    private readonly IStreamSerializer<FPlane> _planeSerializer;

    public DefaultBspNodeSerializer(IStreamSerializer<FPlane> planeSerializer)
    {
        _planeSerializer = planeSerializer;
    }

    /// <inheritdoc />
    public FBspNode Deserialize(Stream stream)
    {
        return new FBspNode
        {
            plane = _planeSerializer.Deserialize(stream),
            iVertPool = stream.ReadInt32(),
            iSurf = stream.ReadInt32(),
            iVertexIndex = stream.ReadInt32(),
            ComponentIndex = stream.ReadUInt16(),
            ComponentNodeIndex = stream.ReadUInt16(),
            ComponentElementIndex = stream.ReadInt32(),
            iChild = stream.ReadInt32s(3),
            iCollisionBound = stream.ReadInt32(),
            iZone = stream.ReadBytes(2),
            NumVertices = (byte) stream.ReadByte(),
            NodeFlags = (byte) stream.ReadByte(),
            iLeaf = stream.ReadInt32s(2)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FBspNode value)
    {
        _planeSerializer.Serialize(stream, value.plane);
        stream.WriteInt32(value.iVertPool);
        stream.WriteInt32(value.iSurf);
        stream.WriteInt32(value.iVertexIndex);
        stream.WriteUInt16(value.ComponentIndex);
        stream.WriteUInt16(value.ComponentNodeIndex);
        stream.WriteInt32(value.ComponentElementIndex);
        stream.WriteInt32s(value.iChild);
        stream.WriteInt32(value.iCollisionBound);
        stream.WriteBytes(value.iZone);
        stream.WriteByte(value.NumVertices);
        stream.WriteByte(value.NodeFlags);
        stream.WriteInt32s(value.iLeaf);
    }
}

public class DefaultBspSurfSerializer : BaseObjectSerializer<FBspSurf>
{
    private readonly IStreamSerializer<FPlane> _planeSerializer;

    public DefaultBspSurfSerializer(IStreamSerializer<FPlane> planeSerializer)
    {
        _planeSerializer = planeSerializer;
    }


    /// <inheritdoc />
    public override void DeserializeObject(FBspSurf obj, IUnrealPackageStream objectStream)
    {
        obj.Material = objectStream.ReadObject() as UMaterialInterface;
        obj.PolyFlags = objectStream.ReadUInt32();
        obj.pBase = objectStream.ReadInt32();
        obj.vNormal = objectStream.ReadInt32();
        obj.vTextureU = objectStream.ReadInt32();
        obj.vTextureV = objectStream.ReadInt32();
        obj.iBrushPoly = objectStream.ReadInt32();
        obj.Actor = objectStream.ReadObject();
        obj.plane = _planeSerializer.Deserialize(objectStream.BaseStream);
        obj.ShadowMapScale = objectStream.ReadSingle();
        obj.LightingChannels = objectStream.ReadUInt32();
        obj.iLightmassIndex = objectStream.ReadInt32();
    }

    /// <inheritdoc />
    public override void SerializeObject(FBspSurf obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteObject(obj.Material);
        objectStream.WriteUInt32(obj.PolyFlags);
        objectStream.WriteInt32(obj.pBase);
        objectStream.WriteInt32(obj.vNormal);
        objectStream.WriteInt32(obj.vTextureU);
        objectStream.WriteInt32(obj.vTextureV);
        objectStream.WriteInt32(obj.iBrushPoly);
        objectStream.WriteObject(obj.Actor);
        _planeSerializer.Serialize(objectStream.BaseStream, obj.plane);
        objectStream.WriteSingle(obj.ShadowMapScale);
        objectStream.WriteUInt32(obj.LightingChannels);
        objectStream.WriteInt32(obj.iLightmassIndex);
    }
}

public class DefaultVertSerializer : IStreamSerializer<FVert>
{
    private readonly IStreamSerializer<FVector2D> _vector2DSerializer;

    public DefaultVertSerializer(IStreamSerializer<FVector2D> vector2DSerializer)
    {
        _vector2DSerializer = vector2DSerializer;
    }

    /// <inheritdoc />
    public FVert Deserialize(Stream stream)
    {
        return new FVert
        {
            pVertex = stream.ReadInt32(),
            iSide = stream.ReadInt32(),
            ShadowTexCoord = _vector2DSerializer.Deserialize(stream),
            BackfaceShadowTexCoord = _vector2DSerializer.Deserialize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FVert value)
    {
        stream.WriteInt32(value.pVertex);
        stream.WriteInt32(value.iSide);
        _vector2DSerializer.Serialize(stream, value.ShadowTexCoord);
        _vector2DSerializer.Serialize(stream, value.BackfaceShadowTexCoord);
    }
}

public class DefaultLightmassPrimitiveSettingsSerializer : IStreamSerializer<FLightmassPrimitiveSettings>
{
    /// <inheritdoc />
    public FLightmassPrimitiveSettings Deserialize(Stream stream)
    {
        return new FLightmassPrimitiveSettings
        {
            bUseTwoSidedLighting = stream.ReadInt32(),
            bShadowIndirectOnly = stream.ReadInt32(),
            FullyOccludedSamplesFraction = stream.ReadSingle(),
            bUseEmissiveForStaticLighting = stream.ReadInt32(),
            EmissiveLightFalloffExponent = stream.ReadSingle(),
            EmissiveLightExplicitInfluenceRadius = stream.ReadSingle(),
            EmissiveBoost = stream.ReadSingle(),
            DiffuseBoost = stream.ReadSingle(),
            SpecularBoost = stream.ReadSingle()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FLightmassPrimitiveSettings value)
    {
        stream.WriteInt32(value.bUseTwoSidedLighting);
        stream.WriteInt32(value.bShadowIndirectOnly);
        stream.WriteSingle(value.FullyOccludedSamplesFraction);
        stream.WriteInt32(value.bUseEmissiveForStaticLighting);
        stream.WriteSingle(value.EmissiveLightFalloffExponent);
        stream.WriteSingle(value.EmissiveLightExplicitInfluenceRadius);
        stream.WriteSingle(value.EmissiveBoost);
        stream.WriteSingle(value.DiffuseBoost);
        stream.WriteSingle(value.SpecularBoost);
    }
}

public class DefaultModelVertexSerializer : IStreamSerializer<FModelVertex>
{
    private readonly IStreamSerializer<FVector2D> _vector2DSerializer;

    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultModelVertexSerializer(IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FVector2D> vector2DSerializer)
    {
        _vectorSerializer = vectorSerializer;
        _vector2DSerializer = vector2DSerializer;
    }

    /// <inheritdoc />
    public FModelVertex Deserialize(Stream stream)
    {
        return new FModelVertex
        {
            Position = _vectorSerializer.Deserialize(stream),
            TangentX = stream.ReadUInt32(),
            TangentY = stream.ReadUInt32(),
            TexCoord = _vector2DSerializer.Deserialize(stream),
            ShadowTexCoord = _vector2DSerializer.Deserialize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FModelVertex value)
    {
        _vectorSerializer.Serialize(stream, value.Position);
        stream.WriteUInt32(value.TangentX);
        stream.WriteUInt32(value.TangentY);
        _vector2DSerializer.Serialize(stream, value.TexCoord);
        _vector2DSerializer.Serialize(stream, value.ShadowTexCoord);
    }
}