using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types;

namespace RlUpk.Core.Serialization.Default.Object.Engine.Struct;

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
        objectStream.WriteTArray(obj.ShadowMaps, (stream, o) => stream.WriteObject(o));
        objectStream.WriteTArray(obj.ShadowVertexBuffers, (stream, o) => stream.WriteObject(o));
        _LightMapSerializer.SerializeObject(obj.FLightMapRef, objectStream);
        objectStream.WriteByte(obj.BLoadVertexColorData);
        if (obj.BLoadVertexColorData != 0)
        {
            _colorVertexBufferSerializer.Serialize(objectStream.BaseStream, obj.ColorVertexBuffer);
        }

        objectStream.WriteInt32(obj.UnkInt);
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


    private void DeSerializeLightMap1D(IUnrealPackageStream objStream, FLightMap fLightMap)
    {
        var res = new FLightMap1D();
        fLightMap.LightGuids = _guidSerializer.ReadTArrayToList(objStream.BaseStream);
        res.ActorOwner = objStream.ReadObject();
        res.DirectionalSamples = _bulkDataSerializer.Deserialize(objStream.BaseStream);
        fLightMap.ScaleVectors[0] = _vectorSerializer.Deserialize(objStream.BaseStream);
        fLightMap.ScaleVectors[1] = _vectorSerializer.Deserialize(objStream.BaseStream);
        fLightMap.ScaleVectors[2] = _vectorSerializer.Deserialize(objStream.BaseStream);
        res.SimpleSamples = _bulkDataSerializer.Deserialize(objStream.BaseStream);
        fLightMap.FLightMap1D = res;
    }


    private void SerializeLightMap1D(IUnrealPackageStream objectStream, FLightMap fLightMap)
    {
        var fLightMap1D = fLightMap.FLightMap1D;
        ArgumentNullException.ThrowIfNull(fLightMap1D);
        objectStream.WriteTArray(fLightMap.LightGuids, _guidSerializer);
        objectStream.WriteObject(fLightMap1D.ActorOwner);
        _bulkDataSerializer.Serialize(objectStream.BaseStream, fLightMap1D.DirectionalSamples);
        _vectorSerializer.Serialize(objectStream.BaseStream, fLightMap.ScaleVectors[0]);
        _vectorSerializer.Serialize(objectStream.BaseStream, fLightMap.ScaleVectors[1]);
        _vectorSerializer.Serialize(objectStream.BaseStream, fLightMap.ScaleVectors[2]);
        _bulkDataSerializer.Serialize(objectStream.BaseStream, fLightMap1D.SimpleSamples);
    }

    private void DeSerializeLightMap2D(IUnrealPackageStream objStream, FLightMap fLightMap)
    {
        var map2D = new FLightMap2D();
        fLightMap.LightGuids = _guidSerializer.ReadTArrayToList(objStream.BaseStream);
        for (var i = 0; i < 3; i++)
        {
            map2D.Textures[i] = objStream.ReadObject() as ULightMapTexture2D;
            fLightMap.ScaleVectors[i] = _vectorSerializer.Deserialize(objStream.BaseStream);
        }

        map2D.CoordinateScale = _vector2DSerializer.Deserialize(objStream.BaseStream);
        map2D.CoordinateBias = _vector2DSerializer.Deserialize(objStream.BaseStream);
        fLightMap.FLightMap2D = map2D;
    }

    private void SerializeLightMap2D(IUnrealPackageStream objectStream, FLightMap fLightMap)
    {
        var fLightMap2D = fLightMap.FLightMap2D;
        ArgumentNullException.ThrowIfNull(fLightMap2D);

        objectStream.WriteTArray(fLightMap.LightGuids, _guidSerializer);
        for (var i = 0; i < 3; i++)
        {
            objectStream.WriteObject(fLightMap2D.Textures[i]);
            _vectorSerializer.Serialize(objectStream.BaseStream, fLightMap.ScaleVectors[i]);
        }

        _vector2DSerializer.Serialize(objectStream.BaseStream, fLightMap2D.CoordinateScale);
        _vector2DSerializer.Serialize(objectStream.BaseStream, fLightMap2D.CoordinateScale);
    }

    /// <inheritdoc />
    public override void DeserializeObject(FLightMap obj, IUnrealPackageStream objectStream)
    {
        // TODO: Refactor - I don't think this modifies the original object passed in
        var type = (FLightMap.LightMapType) objectStream.ReadUInt32();
        switch (type)
        {
            case FLightMap.LightMapType.None:
                return;
            case FLightMap.LightMapType.LightMap1D:
                DeSerializeLightMap1D(objectStream, obj);
                return;
            case FLightMap.LightMapType.LightMap2D:
                DeSerializeLightMap2D(objectStream, obj);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(FLightMap.LightMapType));
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(FLightMap obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteUInt32((uint) obj.Type);
        switch (obj.Type)
        {
            case FLightMap.LightMapType.None:
                return;
            case FLightMap.LightMapType.LightMap1D:
                SerializeLightMap1D(objectStream, obj);
                return;
            case FLightMap.LightMapType.LightMap2D:
                SerializeLightMap2D(objectStream, obj);
                return;
            default:
                throw new ArgumentOutOfRangeException(nameof(FLightMap.LightMapType));
        }
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override void SerializeObject(AStaticLightCollectionActor obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        foreach (var objLightComponentMatrix in obj.LightComponentMatrixes)
        {
            _matrixSerializer.Serialize(objectStream.BaseStream, objLightComponentMatrix);
        }
    }
}

public class DefaulColorVertexBufferSerializer : IStreamSerializer<FColorVertexBuffer>
{
    private readonly IStreamSerializer<FColor> _colorSerializer;


    public DefaulColorVertexBufferSerializer(IStreamSerializer<FColor> colorSerializer)
    {
        _colorSerializer = colorSerializer;
    }

    /// <inheritdoc />
    public FColorVertexBuffer Deserialize(Stream stream)
    {
        return new FColorVertexBuffer
        {
            Stride = stream.ReadUInt32(),
            NumVertices = stream.ReadUInt32(),
            colorStream = _colorSerializer.ReadTArrayWithElementSize(stream)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FColorVertexBuffer value)
    {
        stream.WriteUInt32(value.Stride);
        stream.WriteUInt32(value.NumVertices);
        stream.BulkWriteTArray(value.colorStream, (stream1, color) => _colorSerializer.Serialize(stream1, color));
    }
}