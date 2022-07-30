using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultAnimSequenceSerializer : BaseObjectSerializer<UAnimSequence>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultAnimSequenceSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UAnimSequence obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.RawAnimationDataCount = objectStream.ReadInt32();
        if (obj.RawAnimationDataCount > 0)
        {
            Debugger.Break();
        }

        obj.NumBytes = objectStream.ReadInt32();
        if (obj.NumBytes > 0)
        {
            obj.SerializedData = objectStream.ReadBytes(obj.NumBytes);
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UAnimSequence obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteInt32(obj.RawAnimationDataCount);
        objectStream.WriteInt32(obj.NumBytes);
        if (obj.NumBytes > 0)
        {
            objectStream.WriteBytes(obj.SerializedData);
        }
    }
}