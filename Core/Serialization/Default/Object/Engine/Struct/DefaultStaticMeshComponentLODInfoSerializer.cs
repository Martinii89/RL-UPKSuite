using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultStaticMeshComponentLODInfoSerializer : BaseObjectSerializer<FStaticMeshComponentLODInfo>
{
    private readonly IStreamSerializer<FColorVertexBuffer> _colorVertexBufferSerializer;
    private readonly IObjectSerializer<FLightMap> _LightMapSerializer;

    public DefaultStaticMeshComponentLODInfoSerializer(IStreamSerializer<FGuid> guidSerializer, IObjectSerializer<FLightMap> lightMapSerializer,
        IStreamSerializer<FColorVertexBuffer> colorVertexBufferSerializer)
    {
        _LightMapSerializer = lightMapSerializer;
        _colorVertexBufferSerializer = colorVertexBufferSerializer;
    }


    /// <inheritdoc />
    public override void DeserializeObject(FStaticMeshComponentLODInfo obj, IUnrealPackageStream objectStream)
    {
        obj.ShadowMaps = objectStream.ReadTArray(stream => stream.ReadObject());
        obj.ShadowVertexBuffers = objectStream.ReadTArray(stream => stream.ReadObject());
        // TODO refactor to a "ref object" so the serializer can create a LightMap1D or LightMap2D as a field
        _LightMapSerializer.DeserializeObject(obj.FLightMapRef, objectStream);
        obj.BLoadVertexColorData = objectStream.ReadByte();
        if (obj.BLoadVertexColorData != 0)
        {
            obj.ColorVertexBuffer = _colorVertexBufferSerializer.Deserialize(objectStream.BaseStream);
        }

        obj.UnkInt = objectStream.ReadInt32();
    }

    /// <inheritdoc />
    public override void SerializeObject(FStaticMeshComponentLODInfo obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}

public class DefaultLightMapSerializer : BaseObjectSerializer<FLightMap>
{
    private readonly IStreamSerializer<FByteBulkData> _bulkDataSerializer;
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FVector2D> _vector2DSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultLightMapSerializer(IStreamSerializer<FGuid> guidSerializer,
        IStreamSerializer<FByteBulkData> bulkDataSerializer, IStreamSerializer<FVector> vectorSerializer,
        IStreamSerializer<FVector2D> vector2DSerializer)
    {
        _guidSerializer = guidSerializer;
        _bulkDataSerializer = bulkDataSerializer;
        _vectorSerializer = vectorSerializer;
        _vector2DSerializer = vector2DSerializer;
    }


    private FLightMap1D DeSerializeLightMap1D(IUnrealPackageStream objStream)
    {
        var res = new FLightMap1D();
        res.Type = FLightMap.LightMapType.LightMap1D;
        res.LightGuids = _guidSerializer.ReadTArrayToList(objStream.BaseStream);
        res.ActorOwner = objStream.ReadObject();
        res.DirectionalSamples = _bulkDataSerializer.Deserialize(objStream.BaseStream);
        res.ScaleVectors[0] = _vectorSerializer.Deserialize(objStream.BaseStream);
        res.ScaleVectors[1] = _vectorSerializer.Deserialize(objStream.BaseStream);
        res.ScaleVectors[2] = _vectorSerializer.Deserialize(objStream.BaseStream);
        res.SimpleSamples = _bulkDataSerializer.Deserialize(objStream.BaseStream);
        return res;
    }

    private FLightMap2D DeSerializeLightMap2D(IUnrealPackageStream objStream)
    {
        var map2D = new FLightMap2D();
        map2D.Type = FLightMap.LightMapType.LightMap2D;
        map2D.LightGuids = _guidSerializer.ReadTArrayToList(objStream.BaseStream);
        for (var i = 0; i < 3; i++)
        {
            map2D.Textures[i] = objStream.ReadObject() as ULightMapTexture2D;
            map2D.ScaleVectors[i] = _vectorSerializer.Deserialize(objStream.BaseStream);
        }

        map2D.CoordinateScale = _vector2DSerializer.Deserialize(objStream.BaseStream);
        map2D.CoordinateBias = _vector2DSerializer.Deserialize(objStream.BaseStream);


        return map2D;
    }

    public override void DeserializeObject(FLightMap obj, IUnrealPackageStream objectStream)
    {
        // TODO: Refactor - I don't think this modifies the original object passed in
        var type = (FLightMap.LightMapType) objectStream.ReadUInt32();
        switch (type)
        {
            case FLightMap.LightMapType.None:
                return;
            case FLightMap.LightMapType.LightMap1D:
                obj = DeSerializeLightMap1D(objectStream);
                return;
            case FLightMap.LightMapType.LightMap2D:
                obj = DeSerializeLightMap2D(objectStream);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(FLightMap.LightMapType));
        }
    }

    public override void SerializeObject(FLightMap obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}

public class DefaultStaticLightCollectionActorSerializer : BaseObjectSerializer<AStaticLightCollectionActor>
{
    private readonly IStreamSerializer<FMatrix> _matrixSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;


    public DefaultStaticLightCollectionActorSerializer(IStreamSerializer<FMatrix> matrixSerializer, IObjectSerializer<UObject> objectSerializer)
    {
        _matrixSerializer = matrixSerializer;
        _objectSerializer = objectSerializer;
    }

    public override void DeserializeObject(AStaticLightCollectionActor obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        var lightComponentsProperty = obj.ScriptProperties.Find(x => x.Name == "LightComponents");
        var lightComponents = lightComponentsProperty?.Value as List<object> ?? new List<object>();
        foreach (var lightComponent in lightComponents)
        {
            obj.LightComponentMatrixes.Add(_matrixSerializer.Deserialize(objectStream.BaseStream));
        }
    }

    public override void SerializeObject(AStaticLightCollectionActor obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}

public class DefaulColorVertexBufferSerializer : IStreamSerializer<FColorVertexBuffer>
{
    private readonly IStreamSerializer<FColor> _colorSerializer;


    public DefaulColorVertexBufferSerializer(IStreamSerializer<FColor> colorSerializer)
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