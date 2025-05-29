using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultApexGenericAssetSerializer : BaseObjectSerializer<UApexGenericAsset>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultApexGenericAssetSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UApexGenericAsset obj, IUnrealPackageStream objectStream)
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
    public override void SerializeObject(UApexGenericAsset obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}