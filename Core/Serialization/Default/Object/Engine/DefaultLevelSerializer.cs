using System.Diagnostics;
using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultLevelSerializer : BaseObjectSerializer<ULevel>
{
    private readonly IStreamSerializer<FKCachedConvexData> _kCachedConvexDataSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializer<FPrecomputedLightVolume> _precomputedLightVolumeSerializer;
    private readonly IStreamSerializer<FPrecomputedVisibilityHandler> _precomputedVisibilityHandlerSerializer;
    private readonly IStreamSerializer<FPrecomputedVolumeDistanceField> _precomputedVolumeDistanceFieldSerializer;
    private readonly IStreamSerializer<FURL> _urlSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultLevelSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer,
        IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FURL> urlSerializer,
        IStreamSerializer<FKCachedConvexData> kCachedConvexDataSerializer, IStreamSerializer<FPrecomputedLightVolume> precomputedLightVolumeSerializer,
        IStreamSerializer<FPrecomputedVisibilityHandler> precomputedVisibilityHandlerSerializer,
        IStreamSerializer<FPrecomputedVolumeDistanceField> precomputedVolumeDistanceFieldSerializer)
    {
        _objectSerializer = objectSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _vectorSerializer = vectorSerializer;
        _urlSerializer = urlSerializer;
        _kCachedConvexDataSerializer = kCachedConvexDataSerializer;
        _precomputedLightVolumeSerializer = precomputedLightVolumeSerializer;
        _precomputedVisibilityHandlerSerializer = precomputedVisibilityHandlerSerializer;
        _precomputedVolumeDistanceFieldSerializer = precomputedVolumeDistanceFieldSerializer;
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
        var size = objectStream.ReadInt32();
        objectStream.Move(size);
        obj.CachedPhysBSPData = objectStream.ReadTarrayWithElementSize(stream => (byte) stream.ReadByte());
        ReadCachedPhysSMDataMap(obj, objectStream);
        obj.CachedPhysSMDataStore = _kCachedConvexDataSerializer.ReadTArrayToList(objectStream);
        ReadCachedPhysPerTriSMDataMap(obj, objectStream);
        obj.CachedPhysPerTriSMDataStore = objectStream.ReadTarray(stream =>
        {
            return new FKCachedPerTriData
            {
                CachedPerTriData = stream.ReadTarrayWithElementSize(stream => (byte) stream.ReadByte())
            };
        });
        obj.CachedPhysBSPDataVersion = objectStream.ReadInt32();
        obj.CachedPhysSMDataVersion = objectStream.ReadInt32();
        ReadForceStreamTextures(obj, objectStream);
        obj.CachedPhysConvexBSPData = objectStream.ReadTarray(stream =>
        {
            return new FKCachedConvexDataElement
            {
                ConvexElementData = stream.ReadTarrayWithElementSize(stream => (byte) stream.ReadByte())
            };
        });
        obj.CachedPhysConvexBSPVersion = objectStream.ReadInt32();
        obj.NavListStart = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.NavListEnd = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.CoverListStart = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.CoverListEnd = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.PylonListStart = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.PylonListEnd = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.UnkArrayCountOf20Bytes = objectStream.ReadInt32();
        if (obj.UnkArrayCountOf20Bytes > 0)
        {
            Debugger.Break();
            objectStream.Move(obj.UnkArrayCountOf20Bytes * 20);
        }

        obj.SomeObjectArray = objectStream.ReadTarray(stream => obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)));
        obj.SomeObjectBytePairArray = objectStream.ReadTarray(stream =>
            new Tuple<UObject?, byte>(obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)), (byte) objectStream.ReadByte()));
        obj.CrossLevelActors = objectStream.ReadTarray(stream => obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)));
        obj.FPrecomputedLightVolume = _precomputedLightVolumeSerializer.Deserialize(objectStream);
        obj.PrecomputedLightVolume = _precomputedVisibilityHandlerSerializer.Deserialize(objectStream);
        obj.PrecomputedVolumeDistanceField = _precomputedVolumeDistanceFieldSerializer.Deserialize(objectStream);
    }

    private void ReadForceStreamTextures(ULevel obj, Stream objectStream)
    {
        UTexture KeyRead(Stream stream)
        {
            var mesh = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(stream)) as UTexture;
            ArgumentNullException.ThrowIfNull(mesh);
            return mesh;
        }

        obj.ForceStreamTextures = objectStream.ReadDictionary(KeyRead, stream => stream.ReadInt32() == 1);
    }

    private void ReadCachedPhysPerTriSMDataMap(ULevel obj, Stream objectStream)
    {
        UStaticMesh KeyRead(Stream stream)
        {
            var mesh = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(stream)) as UStaticMesh;
            ArgumentNullException.ThrowIfNull(mesh);
            return mesh;
        }

        FCachedPerTriPhysSMData ValRead(Stream stream)
        {
            return new FCachedPerTriPhysSMData
            {
                Scale3d = _vectorSerializer.Deserialize(stream),
                CachedDataIndex = stream.ReadInt32()
            };
        }

        obj.CachedPhysPerTriSMDataMap = objectStream.ReadTMap(KeyRead, ValRead);
    }

    private void ReadCachedPhysSMDataMap(ULevel obj, Stream objectStream)
    {
        UStaticMesh KeyRead(Stream stream)
        {
            var mesh = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(stream)) as UStaticMesh;
            ArgumentNullException.ThrowIfNull(mesh);
            return mesh;
        }

        FCachedPhysSMData ValRead(Stream stream)
        {
            return new FCachedPhysSMData
            {
                Scale3d = _vectorSerializer.Deserialize(stream),
                CachedDataIndex = stream.ReadInt32()
            };
        }

        obj.CachedPhysSMDataMap = objectStream.ReadTMap(KeyRead, ValRead);
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