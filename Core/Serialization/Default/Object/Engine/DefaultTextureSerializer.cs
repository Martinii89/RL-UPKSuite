using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultTextureSerializer : BaseObjectSerializer<UTexture>
{
    private readonly IStreamSerializer<FByteBulkData> _bulkDataSerializer;

    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultTextureSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<FByteBulkData> bulkDataSerializer)
    {
        _objectSerializer = objectSerializer;
        _bulkDataSerializer = bulkDataSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UTexture obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.SourceArt = _bulkDataSerializer.Deserialize(objectStream.BaseStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UTexture obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        _bulkDataSerializer.Serialize(objectStream.BaseStream, obj.SourceArt);
    }
}