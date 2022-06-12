using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultStaticMeshComponentLODInfoSerializer : IStreamSerializerFor<FStaticMeshComponentLODInfo>
{
    private readonly IStreamSerializerFor<FColorVertexBuffer> _colorVertexBufferSerializer;

    //private readonly IStreamSerializerFor<FColorVertexBuffer> _ColorVertexBufferSerializer;
    private readonly IStreamSerializerFor<FLightMap> _LightMapSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objecIndexSerializer;

    public DefaultStaticMeshComponentLODInfoSerializer(IStreamSerializerFor<FGuid> guidSerializer, IStreamSerializerFor<ObjectIndex> objecIndexSerializer,
        IStreamSerializerFor<FLightMap> lightMapSerializer, IStreamSerializerFor<FColorVertexBuffer> colorVertexBufferSerializer)
    {
        _objecIndexSerializer = objecIndexSerializer;
        _LightMapSerializer = lightMapSerializer;
        _colorVertexBufferSerializer = colorVertexBufferSerializer;
    }

    public FStaticMeshComponentLODInfo Deserialize(Stream stream)
    {
        var obj = new FStaticMeshComponentLODInfo();
        obj.ShadowMaps = _objecIndexSerializer.ReadTArrayToList(stream);
        obj.ShadowVertexBuffers = _objecIndexSerializer.ReadTArrayToList(stream);
        obj.FLightMapRef = _LightMapSerializer.Deserialize(stream);
        obj.BLoadVertexColorData = (byte) stream.ReadByte();
        if (obj.BLoadVertexColorData != 0)
        {
            obj.ColorVertexBuffer = _colorVertexBufferSerializer.Deserialize(stream);
        }

        obj.UnkInt = stream.ReadInt32();
        return obj;
    }

    public void Serialize(Stream stream, FStaticMeshComponentLODInfo value)
    {
        throw new NotImplementedException();
    }
}

public class DefaultLightMapSerializer : IStreamSerializerFor<FLightMap>
{
    private readonly IStreamSerializerFor<FByteBulkData> _bulkDataSerializer;

    private readonly IStreamSerializerFor<FGuid> _guidSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objecIndexSerializer;
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;

    public DefaultLightMapSerializer(IStreamSerializerFor<FGuid> guidSerializer, IStreamSerializerFor<ObjectIndex> objecIndexSerializer,
        IStreamSerializerFor<FByteBulkData> bulkDataSerializer, IStreamSerializerFor<FVector> vectorSerializer)
    {
        _guidSerializer = guidSerializer;
        _objecIndexSerializer = objecIndexSerializer;
        _bulkDataSerializer = bulkDataSerializer;
        _vectorSerializer = vectorSerializer;
    }

    public FLightMap? Deserialize(Stream stream)
    {
        var type = (FLightMap.LightMapType) stream.ReadUInt32();
        switch (type)
        {
            case FLightMap.LightMapType.None:
                return null;
            case FLightMap.LightMapType.LightMap1D:
                return DeSerializeLightMap1D(stream);
            case FLightMap.LightMapType.LightMap2D:
                return DeSerializeLightMap2D(stream);
            default:
                throw new ArgumentOutOfRangeException(nameof(FLightMap.LightMapType));
        }
    }

    public void Serialize(Stream stream, FLightMap value)
    {
        throw new NotImplementedException();
    }

    private FLightMap1D DeSerializeLightMap1D(Stream objStream)
    {
        var res = new FLightMap1D();
        res.Type = FLightMap.LightMapType.LightMap1D;
        res.LightGuids = _guidSerializer.ReadTArrayToList(objStream);
        res.ActorOwner = _objecIndexSerializer.Deserialize(objStream);
        res.DirectionalSamples = _bulkDataSerializer.Deserialize(objStream);
        res.ScaleVectors[0] = _vectorSerializer.Deserialize(objStream);
        res.ScaleVectors[1] = _vectorSerializer.Deserialize(objStream);
        res.ScaleVectors[2] = _vectorSerializer.Deserialize(objStream);
        res.SimpleSamples = _bulkDataSerializer.Deserialize(objStream);
        return res;
    }

    private FLightMap2D DeSerializeLightMap2D(Stream objStream)
    {
        Debugger.Break();
        return new FLightMap2D();
    }
}

public class DefaultStaticLightCollectionActorSerializer : BaseObjectSerializer<AStaticLightCollectionActor>
{
    private readonly IStreamSerializerFor<FMatrix> _matrixSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;


    public DefaultStaticLightCollectionActorSerializer(IStreamSerializerFor<FMatrix> matrixSerializer, IObjectSerializer<UObject> objectSerializer)
    {
        _matrixSerializer = matrixSerializer;
        _objectSerializer = objectSerializer;
    }

    public override void DeserializeObject(AStaticLightCollectionActor obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        var lightComponentsProperty = obj.ScriptProperties.Find(x => x.Name == "LightComponents");
        var lightComponents = lightComponentsProperty?.Value as List<object> ?? new List<object>();
        foreach (var lightComponent in lightComponents)
        {
            obj.LightComponentMatrixes.Add(_matrixSerializer.Deserialize(objectStream));
        }
    }

    public override void SerializeObject(AStaticLightCollectionActor obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}

public class DefaulColorVertexBufferSerializer : IStreamSerializerFor<FColorVertexBuffer>
{
    private readonly IStreamSerializerFor<FColor> _colorSerializer;


    public DefaulColorVertexBufferSerializer(IStreamSerializerFor<FColor> colorSerializer)
    {
        _colorSerializer = colorSerializer;
    }

    public FColorVertexBuffer Deserialize(Stream stream)
    {
        return new FColorVertexBuffer
        {
            Stride = stream.ReadUInt32(),
            NumVertices = stream.ReadUInt32(),
            colorStream = _colorSerializer.ReadTArrayWithElementSize(stream)
        };
    }

    public void Serialize(Stream stream, FColorVertexBuffer value)
    {
        throw new NotImplementedException();
    }
}