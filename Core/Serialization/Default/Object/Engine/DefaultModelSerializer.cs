using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultModelSerializer : BaseObjectSerializer<UModel>
{
    private readonly IStreamSerializer<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializer<FBspNode> _bspNodeSerializer;
    private readonly IStreamSerializer<FBspSurf> _bspSurfSerializer;
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FLightmassPrimitiveSettings> _lightmassPrimitiveSettingsSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;
    private readonly IStreamSerializer<FVert> _vertSerializer;

    public DefaultModelSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FBspNode> bspNodeSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer, IStreamSerializer<FBspSurf> bspSurfSerializer, IStreamSerializer<FVert> vertSerializer,
        IStreamSerializer<FGuid> guidSerializer, IStreamSerializer<FLightmassPrimitiveSettings> lightmassPrimitiveSettingsSerializer)
    {
        _objectSerializer = objectSerializer;
        _boxSphereBoundsSerializer = boxSphereBoundsSerializer;
        _vectorSerializer = vectorSerializer;
        _bspNodeSerializer = bspNodeSerializer;
        _objectIndexSerializer = objectIndexSerializer;
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
        obj.Vectors = objectStream.BaseStream.ReadTarrayWithElementSize(stream => _vectorSerializer.Deserialize(stream));
        obj.Points = objectStream.BaseStream.ReadTarrayWithElementSize(stream => _vectorSerializer.Deserialize(stream));
        obj.Nodes = objectStream.BaseStream.ReadTarrayWithElementSize(stream => _bspNodeSerializer.Deserialize(stream));

        obj.Surfs.Super = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream));
        obj.Surfs.Data = objectStream.BaseStream.ReadTarray(stream => _bspSurfSerializer.Deserialize(stream));
        obj.Verts = _vertSerializer.ReadTArrayWithElementSize(objectStream.BaseStream);
        obj.NumSharedSides = objectStream.ReadInt32();
        obj.NumZones = objectStream.ReadInt32();
        if (obj.NumZones > 0)
        {
            Debugger.Break();
        }

        obj.Polys = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream));
        obj.LeafHulls = objectStream.BaseStream.ReadTarrayWithElementSize(stream => stream.ReadInt32());
        obj.Leaves = objectStream.BaseStream.ReadTarrayWithElementSize(stream => stream.ReadInt32());
        obj.RootOutside = objectStream.ReadUInt32();
        obj.Linked = objectStream.ReadUInt32();
        obj.PortalNodes = objectStream.BaseStream.ReadTarrayWithElementSize(stream => stream.ReadInt32());
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

public class DefaultBspSurfSerializer : IStreamSerializer<FBspSurf>
{
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;
    private readonly IStreamSerializer<FPlane> _planeSerializer;

    public DefaultBspSurfSerializer(IStreamSerializer<ObjectIndex> objectIndexSerializer, IStreamSerializer<FPlane> planeSerializer)
    {
        _objectIndexSerializer = objectIndexSerializer;
        _planeSerializer = planeSerializer;
    }

    public FBspSurf Deserialize(Stream stream)
    {
        return new FBspSurf
        {
            Material = _objectIndexSerializer.Deserialize(stream),
            PolyFlags = stream.ReadUInt32(),
            pBase = stream.ReadInt32(),
            vNormal = stream.ReadInt32(),
            vTextureU = stream.ReadInt32(),
            vTextureV = stream.ReadInt32(),
            iBrushPoly = stream.ReadInt32(),
            Actor = _objectIndexSerializer.Deserialize(stream),
            plane = _planeSerializer.Deserialize(stream),
            ShadowMapScale = stream.ReadSingle(),
            LightingChannels = stream.ReadUInt32(),
            iLightmassIndex = stream.ReadInt32()
        };
    }

    public void Serialize(Stream stream, FBspSurf value)
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