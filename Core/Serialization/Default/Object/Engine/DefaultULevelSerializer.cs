using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultULevelSerializer : BaseObjectSerializer<ULevel>
{
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializerFor<FURL> _urlSerializer;
    private readonly IStreamSerializerFor<FVector> _vectorSerializer;

    public DefaultULevelSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerializer,
        IStreamSerializerFor<FVector> vectorSerializer, IStreamSerializerFor<FURL> urlSerializer)
    {
        _objectSerializer = objectSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _vectorSerializer = vectorSerializer;
        _urlSerializer = urlSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(ULevel obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        obj.Actors.Super = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.Actors.Data = objectStream.ReadTarray(stream => obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)));
        obj.URL = _urlSerializer.Deserialize(objectStream);
        obj.Model = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.ModelComponents = objectStream.ReadTarray(stream => obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)));
        obj.GameSequences = objectStream.ReadTarray(stream => obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)));
        ReadTextureToInstancesMap(obj, objectStream);
        ReadDynamicTextureInstances(obj, objectStream);
    }

    private void ReadDynamicTextureInstances(ULevel obj, Stream objectStream)
    {
        UComponent KeyRead(Stream stream)
        {
            var component = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(stream)) as UComponent;
            ArgumentNullException.ThrowIfNull(component);
            return component;
        }

        List<FDynamicTextureInstance> ValRead(Stream stream)
        {
            return stream.ReadTarray(stream1 => new FDynamicTextureInstance
            {
                Center = _vectorSerializer.Deserialize(stream),
                W = stream1.ReadInt32(),
                TexelFactor = stream1.ReadSingle(),
                Tex = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(stream1)) as UTexture,
                BAttached = stream1.ReadInt32(),
                OriginalRadius = stream1.ReadSingle()
            });
        }

        obj.DynamicTextureInstances = objectStream.ReadDictionary(KeyRead, ValRead);
    }

    private void ReadTextureToInstancesMap(ULevel obj, Stream objectStream)
    {
        UTexture KeyRead(Stream stream)
        {
            var tex = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(stream)) as UTexture;
            ArgumentNullException.ThrowIfNull(tex);
            return tex;
        }

        List<FStreamableTextureInstance> ValRead(Stream stream)
        {
            return stream.ReadTarray(stream1 => new FStreamableTextureInstance
            {
                Center = _vectorSerializer.Deserialize(stream),
                W = stream1.ReadInt32(),
                TexelFactor = stream1.ReadSingle()
            });
        }

        obj.TextureToInstancesMap = objectStream.ReadDictionary(KeyRead, ValRead);
    }

    /// <inheritdoc />
    public override void SerializeObject(ULevel obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}

public class DefaultFURLSerialize : IStreamSerializerFor<FURL>
{
    public FURL Deserialize(Stream stream)
    {
        return new FURL
        {
            Protocol = stream.ReadFString(),
            Host = stream.ReadFString(),
            Map = stream.ReadFString(),
            Portal = stream.ReadFString(),
            Op = stream.ReadTarray(stream1 => stream1.ReadFString()),
            Port = stream.ReadInt32(),
            Valid = stream.ReadInt32()
        };
    }

    public void Serialize(Stream stream, FURL value)
    {
        throw new NotImplementedException();
    }
}