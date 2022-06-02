using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultTexture2DSerializer : BaseObjectSerializer<UTexture2D>
{
    private readonly IStreamSerializerFor<FByteBulkData> _bulkDataSerializer;
    private readonly IStreamSerializerFor<FGuid> _guidSerializer;
    private readonly IStreamSerializerFor<Mip> _mipSerializer;

    private readonly IObjectSerializer<UTexture> _textureSerializer;

    public DefaultTexture2DSerializer(IObjectSerializer<UTexture> textureSerializer, IStreamSerializerFor<Mip> mipSerializer,
        IStreamSerializerFor<FGuid> guidSerializer, IStreamSerializerFor<FByteBulkData> bulkDataSerializer)
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