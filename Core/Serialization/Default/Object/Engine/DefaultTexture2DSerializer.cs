using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultTexture2DSerializer : BaseObjectSerializer<UTexture2D>
{
    private readonly IStreamSerializer<FByteBulkData> _bulkDataSerializer;
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<Mip> _mipSerializer;

    private readonly IObjectSerializer<UTexture> _textureSerializer;

    public DefaultTexture2DSerializer(IObjectSerializer<UTexture> textureSerializer, IStreamSerializer<Mip> mipSerializer,
        IStreamSerializer<FGuid> guidSerializer, IStreamSerializer<FByteBulkData> bulkDataSerializer)
    {
        _textureSerializer = textureSerializer;
        _mipSerializer = mipSerializer;
        _guidSerializer = guidSerializer;
        _bulkDataSerializer = bulkDataSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UTexture2D obj, Stream objectStream)
    {
        _textureSerializer.DeserializeObject(obj, objectStream);
        _mipSerializer.ReadTArrayToList(objectStream, obj.Mips);
        obj.TextureFileCacheGuid = _guidSerializer.Deserialize(objectStream);
        _mipSerializer.ReadTArrayToList(objectStream, obj.CachedPVRTCMips);
        obj.CachedFlashMipsMaxResolution = objectStream.ReadInt32();
        _mipSerializer.ReadTArrayToList(objectStream, obj.CachedATITCMips);
        obj.CachedFlashMips = _bulkDataSerializer.Deserialize(objectStream);
        _mipSerializer.ReadTArrayToList(objectStream, obj.CachedETCMips);
    }

    /// <inheritdoc />
    public override void SerializeObject(UTexture2D obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}