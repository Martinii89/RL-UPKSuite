using System.Diagnostics;

using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultLevelSerializer : BaseObjectSerializer<ULevel>
{
    private readonly IStreamSerializer<FKCachedConvexData> _kCachedConvexDataSerializer;
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IStreamSerializer<FPrecomputedLightVolume> _precomputedLightVolumeSerializer;
    private readonly IStreamSerializer<FPrecomputedVisibilityHandler> _precomputedVisibilityHandlerSerializer;
    private readonly IStreamSerializer<FPrecomputedVolumeDistanceField> _precomputedVolumeDistanceFieldSerializer;
    private readonly IStreamSerializer<FURL> _urlSerializer;
    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultLevelSerializer(IObjectSerializer<UObject> objectSerializer,
        IStreamSerializer<FVector> vectorSerializer, IStreamSerializer<FURL> urlSerializer,
        IStreamSerializer<FKCachedConvexData> kCachedConvexDataSerializer, IStreamSerializer<FPrecomputedLightVolume> precomputedLightVolumeSerializer,
        IStreamSerializer<FPrecomputedVisibilityHandler> precomputedVisibilityHandlerSerializer,
        IStreamSerializer<FPrecomputedVolumeDistanceField> precomputedVolumeDistanceFieldSerializer)
    {
        _objectSerializer = objectSerializer;
        _vectorSerializer = vectorSerializer;
        _urlSerializer = urlSerializer;
        _kCachedConvexDataSerializer = kCachedConvexDataSerializer;
        _precomputedLightVolumeSerializer = precomputedLightVolumeSerializer;
        _precomputedVisibilityHandlerSerializer = precomputedVisibilityHandlerSerializer;
        _precomputedVolumeDistanceFieldSerializer = precomputedVolumeDistanceFieldSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(ULevel obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        obj.Actors.Super = objectStream.ReadObject();
        obj.Actors.Data = objectStream.ReadTArray(stream => stream.ReadObject());
        obj.URL = _urlSerializer.Deserialize(objectStream.BaseStream);
        obj.Model = objectStream.ReadObject();
        obj.ModelComponents = objectStream.ReadTArray(stream => stream.ReadObject());
        obj.GameSequences = objectStream.ReadTArray(stream => stream.ReadObject());
        ReadTextureToInstancesMap(obj, objectStream);
        ReadDynamicTextureInstances(obj, objectStream);
        // When writing we can just write a zero here
        var size = objectStream.ReadInt32();
        objectStream.BaseStream.Move(size);
        obj.CachedPhysBSPData = objectStream.BulkReadTArray(stream => stream.ReadByte());
        ReadCachedPhysSMDataMap(obj, objectStream);
        obj.CachedPhysSMDataStore = _kCachedConvexDataSerializer.ReadTArrayToList(objectStream.BaseStream);
        ReadCachedPhysPerTriSMDataMap(obj, objectStream);
        obj.CachedPhysPerTriSMDataStore = objectStream.ReadTArray(stream =>
        {
            return new FKCachedPerTriData
            {
                CachedPerTriData = stream.BulkReadTArray(stream => stream.ReadByte())
            };
        });
        obj.CachedPhysBSPDataVersion = objectStream.ReadInt32();
        obj.CachedPhysSMDataVersion = objectStream.ReadInt32();
        ReadForceStreamTextures(obj, objectStream);
        obj.CachedPhysConvexBSPData = objectStream.ReadTArray(stream =>
        {
            return new FKCachedConvexDataElement
            {
                ConvexElementData = stream.BulkReadTArray(stream => stream.ReadByte())
            };
        });
        obj.CachedPhysConvexBSPVersion = objectStream.ReadInt32();
        obj.NavListStart = objectStream.ReadObject();
        obj.NavListEnd = objectStream.ReadObject();
        obj.CoverListStart = objectStream.ReadObject();
        obj.CoverListEnd = objectStream.ReadObject();
        obj.PylonListStart = objectStream.ReadObject();
        obj.PylonListEnd = objectStream.ReadObject();
        obj.UnkArrayCountOf20Bytes = objectStream.ReadInt32();
        if (obj.UnkArrayCountOf20Bytes > 0)
        {
            Debugger.Break();
            objectStream.BaseStream.Move(obj.UnkArrayCountOf20Bytes * 20);
        }

        obj.SomeObjectArray = objectStream.ReadTArray(stream => stream.ReadObject());
        obj.SomeObjectBytePairArray = objectStream.ReadTArray(stream => new Tuple<UObject?, byte>(stream.ReadObject(), objectStream.ReadByte()));
        obj.CrossLevelActors = objectStream.ReadTArray(stream => stream.ReadObject());
        obj.FPrecomputedLightVolume = _precomputedLightVolumeSerializer.Deserialize(objectStream.BaseStream);
        obj.PrecomputedLightVolume = _precomputedVisibilityHandlerSerializer.Deserialize(objectStream.BaseStream);
        obj.PrecomputedVolumeDistanceField = _precomputedVolumeDistanceFieldSerializer.Deserialize(objectStream.BaseStream);
    }

    private void ReadForceStreamTextures(ULevel obj, IUnrealPackageStream objectStream)
    {
        UTexture KeyRead(IUnrealPackageStream stream)
        {
            var mesh = objectStream.ReadObject() as UTexture;
            ArgumentNullException.ThrowIfNull(mesh);
            return mesh;
        }

        obj.ForceStreamTextures = objectStream.ReadDictionary(KeyRead, stream => stream.ReadBool());
    }

    private void ReadCachedPhysPerTriSMDataMap(ULevel obj, IUnrealPackageStream objectStream)
    {
        UStaticMesh KeyRead(IUnrealPackageStream stream)
        {
            var mesh = objectStream.ReadObject() as UStaticMesh;
            ArgumentNullException.ThrowIfNull(mesh);
            return mesh;
        }

        FCachedPerTriPhysSMData ValRead(IUnrealPackageStream stream)
        {
            return new FCachedPerTriPhysSMData
            {
                Scale3d = _vectorSerializer.Deserialize(stream.BaseStream),
                CachedDataIndex = stream.ReadInt32()
            };
        }

        obj.CachedPhysPerTriSMDataMap = objectStream.ReadTMap(KeyRead, ValRead);
    }

    private void ReadCachedPhysSMDataMap(ULevel obj, IUnrealPackageStream objectStream)
    {
        UStaticMesh KeyRead(IUnrealPackageStream stream)
        {
            var mesh = objectStream.ReadObject() as UStaticMesh;
            ArgumentNullException.ThrowIfNull(mesh);
            return mesh;
        }

        FCachedPhysSMData ValRead(IUnrealPackageStream stream)
        {
            return new FCachedPhysSMData
            {
                Scale3d = _vectorSerializer.Deserialize(stream.BaseStream),
                CachedDataIndex = stream.ReadInt32()
            };
        }

        obj.CachedPhysSMDataMap = objectStream.ReadTMap(KeyRead, ValRead);
    }

    private void ReadDynamicTextureInstances(ULevel obj, IUnrealPackageStream objectStream)
    {
        UComponent KeyRead(IUnrealPackageStream stream)
        {
            var component = stream.ReadObject() as UComponent;
            ArgumentNullException.ThrowIfNull(component);
            return component;
        }

        List<FDynamicTextureInstance> ValRead(IUnrealPackageStream stream)
        {
            return stream.ReadTArray(stream1 => new FDynamicTextureInstance
            {
                Center = _vectorSerializer.Deserialize(stream1.BaseStream),
                W = stream1.ReadInt32(),
                TexelFactor = stream1.ReadSingle(),
                Tex = stream1.ReadObject() as UTexture,
                BAttached = stream1.ReadInt32(),
                OriginalRadius = stream1.ReadSingle()
            });
        }

        obj.DynamicTextureInstances = objectStream.ReadDictionary(KeyRead, ValRead);
    }

    private void ReadTextureToInstancesMap(ULevel obj, IUnrealPackageStream objectStream)
    {
        UTexture KeyRead(IUnrealPackageStream stream)
        {
            var tex = stream.ReadObject() as UTexture;
            ArgumentNullException.ThrowIfNull(tex);
            return tex;
        }

        List<FStreamableTextureInstance> ValRead(IUnrealPackageStream stream)
        {
            return stream.ReadTArray(stream1 => new FStreamableTextureInstance
            {
                Center = _vectorSerializer.Deserialize(stream1.BaseStream),
                W = stream1.ReadInt32(),
                TexelFactor = stream1.ReadSingle()
            });
        }

        obj.TextureToInstancesMap = objectStream.ReadDictionary(KeyRead, ValRead);
    }

    /// <inheritdoc />
    public override void SerializeObject(ULevel obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteObject(obj.Actors.Super);
        objectStream.WriteTArray(obj.Actors.Data, (stream, val) => stream.WriteObject(val));
        _urlSerializer.Serialize(objectStream.BaseStream, obj.URL);
        objectStream.WriteObject(obj.Model);
        objectStream.WriteTArray(obj.ModelComponents, (stream, val) => stream.WriteObject(val));
        objectStream.WriteTArray(obj.GameSequences, (stream, val) => stream.WriteObject(val));

        objectStream.WriteDictionary(obj.TextureToInstancesMap,
            (stream, texture) => stream.WriteObject(texture),
            (stream, list) => stream.WriteTArray(list, (packageStream, instance) =>
            {
                _vectorSerializer.Serialize(packageStream.BaseStream, instance.Center);
                packageStream.WriteInt32(instance.W);
                packageStream.WriteSingle(instance.TexelFactor);
            }));

        objectStream.WriteDictionary(obj.DynamicTextureInstances,
            (stream, component) => stream.WriteObject(component),
            (stream, list) => stream.WriteTArray(list, (packageStream, instance) =>
            {
                _vectorSerializer.Serialize(packageStream.BaseStream, instance.Center);
                packageStream.WriteInt32(instance.W);
                packageStream.WriteSingle(instance.TexelFactor);
                packageStream.WriteInt32(instance.BAttached);
                packageStream.WriteSingle(instance.OriginalRadius);
            }));
        objectStream.WriteInt32(0); // zero size 
        objectStream.BulkWriteTArray(obj.CachedPhysBSPData, (stream, b) => stream.WriteByte(b)); //easy but probably inefficient

        objectStream.WriteTMap(obj.CachedPhysSMDataMap,
            (stream, mesh) => stream.WriteObject(mesh),
            (stream, data) =>
            {
                _vectorSerializer.Serialize(stream.BaseStream, data.Scale3d);
                stream.WriteInt32(data.CachedDataIndex);
            });

        objectStream.WriteTArray(obj.CachedPhysSMDataStore, _kCachedConvexDataSerializer);

        objectStream.WriteTMap(obj.CachedPhysPerTriSMDataMap,
            (stream, mesh) => stream.WriteObject(mesh),
            (stream, data) =>
            {
                _vectorSerializer.Serialize(stream.BaseStream, data.Scale3d);
                stream.WriteInt32(data.CachedDataIndex);
            });
        objectStream.WriteTArray(obj.CachedPhysPerTriSMDataStore,
            (stream, data) => stream.BulkWriteTArray(data.CachedPerTriData, (stream2, b) => stream2.WriteByte(b)));
        objectStream.WriteInt32(obj.CachedPhysBSPDataVersion);
        objectStream.WriteInt32(obj.CachedPhysSMDataVersion);
        objectStream.WriteDictionary(obj.ForceStreamTextures,
            (stream, texture) => stream.WriteObject(texture),
            (stream, b) => stream.WriteBool(b));
        objectStream.WriteTArray(obj.CachedPhysConvexBSPData,
            (stream, element) => stream.BulkWriteTArray(element.ConvexElementData, (packageStream, b) => packageStream.WriteByte(b)));
        objectStream.WriteInt32(obj.CachedPhysConvexBSPVersion);
        objectStream.WriteObject(obj.NavListStart);
        objectStream.WriteObject(obj.NavListEnd);
        objectStream.WriteObject(obj.CoverListStart);
        objectStream.WriteObject(obj.CoverListEnd);
        objectStream.WriteObject(obj.PylonListStart);
        objectStream.WriteObject(obj.PylonListEnd);
        objectStream.WriteInt32(0); // UnkArrayCountOf20Bytes
        objectStream.WriteTArray(obj.SomeObjectArray, (stream, o) => stream.WriteObject(o));
        objectStream.WriteTArray(obj.SomeObjectBytePairArray, (stream, tuple) =>
        {
            stream.WriteObject(tuple.Item1);
            stream.WriteByte(tuple.Item2);
        });
        objectStream.WriteTArray(obj.CrossLevelActors, (stream, o) => stream.WriteObject(o));
        _precomputedLightVolumeSerializer.Serialize(objectStream.BaseStream, obj.FPrecomputedLightVolume);
        _precomputedVisibilityHandlerSerializer.Serialize(objectStream.BaseStream, obj.PrecomputedLightVolume);
        _precomputedVolumeDistanceFieldSerializer.Serialize(objectStream.BaseStream, obj.PrecomputedVolumeDistanceField);
    }
}