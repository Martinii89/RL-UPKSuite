using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

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
    public override void DeserializeObject(UTexture2D obj, IUnrealPackageStream objectStream)
    {
        _textureSerializer.DeserializeObject(obj, objectStream);
        _mipSerializer.ReadTArrayToList(objectStream.BaseStream, obj.Mips);
        obj.TextureFileCacheGuid = _guidSerializer.Deserialize(objectStream.BaseStream);
        _mipSerializer.ReadTArrayToList(objectStream.BaseStream, obj.CachedPVRTCMips);
        obj.CachedFlashMipsMaxResolution = objectStream.ReadInt32();
        _mipSerializer.ReadTArrayToList(objectStream.BaseStream, obj.CachedATITCMips);
        obj.CachedFlashMips = _bulkDataSerializer.Deserialize(objectStream.BaseStream);
        _mipSerializer.ReadTArrayToList(objectStream.BaseStream, obj.CachedETCMips);
    }

    /// <inheritdoc />
    public override void SerializeObject(UTexture2D obj, IUnrealPackageStream objectStream)
    {
        _textureSerializer.SerializeObject(obj, objectStream);

        _mipSerializer.WriteTArray(objectStream.BaseStream, obj.Mips.Where(x => !x.Data.StoredInSeparateFile).ToArray());
        _guidSerializer.Serialize(objectStream.BaseStream, obj.TextureFileCacheGuid);
        _mipSerializer.WriteTArray(objectStream.BaseStream, obj.CachedPVRTCMips.ToArray());
        objectStream.WriteInt32(obj.CachedFlashMipsMaxResolution);
        _mipSerializer.WriteTArray(objectStream.BaseStream, obj.CachedATITCMips.ToArray());
        _bulkDataSerializer.Serialize(objectStream.BaseStream, obj.CachedFlashMips);
        _mipSerializer.WriteTArray(objectStream.BaseStream, obj.CachedETCMips.ToArray());
    }
}