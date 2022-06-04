using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultApexGenericAssetSerializer : BaseObjectSerializer<UApexGenericAsset>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultApexGenericAssetSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UApexGenericAsset obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        var bAssetValid = objectStream.ReadInt32();
        if (bAssetValid >= 1)
        {
            var nameBuffer = objectStream.ReadFString();
            //int NameBufferSize;
            //byte NameBuffer[NameBufferSize];
            var Size = objectStream.ReadInt32();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UApexGenericAsset obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}