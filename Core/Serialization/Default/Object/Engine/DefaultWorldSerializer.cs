using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultWorldSerializer : BaseObjectSerializer<UWorld>
{
    private readonly IStreamSerializer<FLevelViewportInfo> _levelViewportSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultWorldSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<FLevelViewportInfo> levelViewportSerializer)
    {
        _objectSerializer = objectSerializer;
        _levelViewportSerializer = levelViewportSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UWorld obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.PersistentLevel = objectStream.ReadObject() as ULevel;
        obj.PersistentFaceFXAnimSet = objectStream.ReadObject();
        obj.FLevelViewportInfos[0] = _levelViewportSerializer.Deserialize(objectStream.BaseStream);
        obj.FLevelViewportInfos[1] = _levelViewportSerializer.Deserialize(objectStream.BaseStream);
        obj.FLevelViewportInfos[2] = _levelViewportSerializer.Deserialize(objectStream.BaseStream);
        obj.FLevelViewportInfos[3] = _levelViewportSerializer.Deserialize(objectStream.BaseStream);
        obj.SaveGameSummary_DEPRECATED = objectStream.ReadObject();
        obj.ExtraReferencedObjects = objectStream.ReadTArray(stream => stream.ReadObject());
    }

    /// <inheritdoc />
    public override void SerializeObject(UWorld obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteObject(obj.PersistentLevel);
        objectStream.WriteObject(obj.PersistentFaceFXAnimSet);
        _levelViewportSerializer.Serialize(objectStream.BaseStream, obj.FLevelViewportInfos[0]);
        _levelViewportSerializer.Serialize(objectStream.BaseStream, obj.FLevelViewportInfos[1]);
        _levelViewportSerializer.Serialize(objectStream.BaseStream, obj.FLevelViewportInfos[2]);
        _levelViewportSerializer.Serialize(objectStream.BaseStream, obj.FLevelViewportInfos[3]);
        objectStream.WriteObject(obj.SaveGameSummary_DEPRECATED);
        objectStream.WriteTArray(obj.ExtraReferencedObjects, (stream, o) => stream.WriteObject(o));
    }
}

public class DefaultLevelViewportInfoSerializer : IStreamSerializer<FLevelViewportInfo>
{
    private readonly IStreamSerializer<FRotator> _rotatorSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultLevelViewportInfoSerializer(IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FRotator> rotatorSerializer)
    {
        _vectorSerializer = vectorSerializer;
        _rotatorSerializer = rotatorSerializer;
    }

    /// <inheritdoc />
    public FLevelViewportInfo Deserialize(Stream stream)
    {
        return new FLevelViewportInfo
        {
            CamPosition = _vectorSerializer.Deserialize(stream),
            CamRotation = _rotatorSerializer.Deserialize(stream),
            CamOrthoZoom = stream.ReadSingle()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FLevelViewportInfo value)
    {
        _vectorSerializer.Serialize(stream, value.CamPosition);
        _rotatorSerializer.Serialize(stream, value.CamRotation);
        stream.WriteSingle(value.CamOrthoZoom);
    }
}