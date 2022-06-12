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
    private readonly IStreamSerializerFor<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializerFor<FBspNode> _bspNodeSerializer;
    private readonly IStreamSerializerFor<FBspSurf> _bspSurfSerializer;
    private readonly IStreamSerializerFor<FGuid> _guidSerializer;
    private readonly IStreamSerializerFor<FLightmassPrimitiveSettings> _lightmassPrimitiveSettingsSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;
    private readonly IStreamSerializerFor<FVert> _vertSerializer;

    public DefaultModelSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializerFor<FBoxSphereBounds> boxSphereBoundsSerializer,
        IStreamSerializerFor<FVector> vectorSerializer, IStreamSerializerFor<FBspNode> bspNodeSerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer, IStreamSerializerFor<FBspSurf> bspSurfSerializer, IStreamSerializerFor<FVert> vertSerializer,
        IStreamSerializerFor<FGuid> guidSerializer, IStreamSerializerFor<FLightmassPrimitiveSettings> lightmassPrimitiveSettingsSerializer)
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
    public override void DeserializeObject(UModel obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.Bounds = _boxSphereBoundsSerializer.Deserialize(objectStream);
        obj.Vectors = objectStream.ReadTarrayWithElementSize(stream => _vectorSerializer.Deserialize(stream));
        obj.Points = objectStream.ReadTarrayWithElementSize(stream => _vectorSerializer.Deserialize(stream));
        obj.Nodes = objectStream.ReadTarrayWithElementSize(stream => _bspNodeSerializer.Deserialize(stream));

        obj.Surfs.Super = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.Surfs.Data = objectStream.ReadTarray(stream => _bspSurfSerializer.Deserialize(stream));
        obj.Verts = _vertSerializer.ReadTArrayWithElementSize(objectStream);
        obj.NumSharedSides = objectStream.ReadInt32();
        obj.NumZones = objectStream.ReadInt32();
        if (obj.NumZones > 0)
        {
            Debugger.Break();
        }

        obj.Polys = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.LeafHulls = objectStream.ReadTarrayWithElementSize(stream => stream.ReadInt32());
        obj.Leaves = objectStream.ReadTarrayWithElementSize(stream => stream.ReadInt32());
        obj.RootOutside = objectStream.ReadUInt32();
        obj.Linked = objectStream.ReadUInt32();
        obj.PortalNodes = objectStream.ReadTarrayWithElementSize(stream => stream.ReadInt32());
        obj.NumVertices = objectStream.ReadInt32();
        var elementSize = objectStream.ReadInt32();
        var verticies = objectStream.ReadInt32();
        if (verticies > 0)
        {
            Debugger.Break();
        }

        obj.lightingGuid = _guidSerializer.Deserialize(objectStream);
        obj.LightmassPrimitiveSettings = _lightmassPrimitiveSettingsSerializer.ReadTArrayToList(objectStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UModel obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}

public class DefaultBspNodeSerializer : IStreamSerializerFor<FBspNode>
{
    private readonly IStreamSerializerFor<FPlane> _planeSerializer;

    public DefaultBspNodeSerializer(IStreamSerializerFor<FPlane> planeSerializer)
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

public class DefaultBspSurfSerializer : IStreamSerializerFor<FBspSurf>
{
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;
    private readonly IStreamSerializerFor<FPlane> _planeSerializer;

    public DefaultBspSurfSerializer(IStreamSerializerFor<ObjectIndex> objectIndexSerializer, IStreamSerializerFor<FPlane> planeSerializer)
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

public class DefaultVertSerializer : IStreamSerializerFor<FVert>
{
    private readonly IStreamSerializerFor<FVector2D> _vector2DSerializer;

    public DefaultVertSerializer(IStreamSerializerFor<FVector2D> vector2DSerializer)
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

public class DefaultLightmassPrimitiveSettingsSerializer : IStreamSerializerFor<FLightmassPrimitiveSettings>
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