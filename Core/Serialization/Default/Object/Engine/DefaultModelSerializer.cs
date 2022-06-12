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
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializerFor<FBoxSphereBounds> _boxSphereBoundsSerializer;
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;
    private readonly IStreamSerializerFor<FBspNode> _bspNodeSerializer;
    private readonly IStreamSerializerFor<FBspSurf> _bspSurfSerializer;
    private readonly IStreamSerializerFor<FVert> _vertSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;
    private readonly IStreamSerializerFor<FGuid> _guidSerializer;
    private readonly IStreamSerializerFor<FLightmassPrimitiveSettings> _lightmassPrimitiveSettingsSerializer;

    public DefaultModelSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializerFor<FBoxSphereBounds> boxSphereBoundsSerializer, IStreamSerializerFor<FVector> vectorSerializer, IStreamSerializerFor<FBspNode> bspNodeSerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerializer, IStreamSerializerFor<FBspSurf> bspSurfSerializer, IStreamSerializerFor<FVert> vertSerializer, IStreamSerializerFor<FGuid> guidSerializer, IStreamSerializerFor<FLightmassPrimitiveSettings> lightmassPrimitiveSettingsSerializer)
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
        obj.bounds = _boxSphereBoundsSerializer.Deserialize(objectStream);
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
        obj.Polys =  obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
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

public class DefaultFBspNodeSerializer : IStreamSerializerFor<FBspNode>
{
    public FBspNode Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Serialize(Stream stream, FBspNode value)
    {
        throw new NotImplementedException();
    }
}