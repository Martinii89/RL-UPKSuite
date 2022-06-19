using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultModelSerializer : BaseObjectSerializer<UModel>
{
    private readonly IStreamSerializer<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializer<FBspNode> _bspNodeSerializer;
    private readonly IObjectSerializer<FBspSurf> _bspSurfSerializer;
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FLightmassPrimitiveSettings> _lightmassPrimitiveSettingsSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;
    private readonly IStreamSerializer<FVert> _vertSerializer;

    public DefaultModelSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FBspNode> bspNodeSerializer,
        IObjectSerializer<FBspSurf> bspSurfSerializer, IStreamSerializer<FVert> vertSerializer,
        IStreamSerializer<FGuid> guidSerializer, IStreamSerializer<FLightmassPrimitiveSettings> lightmassPrimitiveSettingsSerializer)
    {
        _objectSerializer = objectSerializer;
        _boxSphereBoundsSerializer = boxSphereBoundsSerializer;
        _vectorSerializer = vectorSerializer;
        _bspNodeSerializer = bspNodeSerializer;
        _bspSurfSerializer = bspSurfSerializer;
        _vertSerializer = vertSerializer;
        _guidSerializer = guidSerializer;
        _lightmassPrimitiveSettingsSerializer = lightmassPrimitiveSettingsSerializer;
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
        var elementSize = objectStream.ReadInt32();
        var verticies = objectStream.ReadInt32();
        if (verticies > 0)
        {
            Debugger.Break();
        }

        obj.lightingGuid = _guidSerializer.Deserialize(objectStream.BaseStream);
        obj.LightmassPrimitiveSettings = _lightmassPrimitiveSettingsSerializer.ReadTArrayToList(objectStream.BaseStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UModel obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}

public class DefaultBspNodeSerializer : IStreamSerializer<FBspNode>
{
    private readonly IStreamSerializer<FPlane> _planeSerializer;

    public DefaultBspNodeSerializer(IStreamSerializer<FPlane> planeSerializer)
    {
        _planeSerializer = planeSerializer;
    }

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

    public void Serialize(Stream stream, FBspNode value)
    {
        throw new NotImplementedException();
    }
}

public class DefaultBspSurfSerializer : BaseObjectSerializer<FBspSurf>
{
    private readonly IStreamSerializer<FPlane> _planeSerializer;

    public DefaultBspSurfSerializer(IStreamSerializer<FPlane> planeSerializer)
    {
        ;
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
        throw new NotImplementedException();
    }
}

public class DefaultVertSerializer : IStreamSerializer<FVert>
{
    private readonly IStreamSerializer<FVector2D> _vector2DSerializer;

    public DefaultVertSerializer(IStreamSerializer<FVector2D> vector2DSerializer)
    {
        _vector2DSerializer = vector2DSerializer;
    }

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

    public void Serialize(Stream stream, FVert value)
    {
        throw new NotImplementedException();
    }
}

public class DefaultLightmassPrimitiveSettingsSerializer : IStreamSerializer<FLightmassPrimitiveSettings>
{
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

    public void Serialize(Stream stream, FLightmassPrimitiveSettings value)
    {
        throw new NotImplementedException();
    }
}