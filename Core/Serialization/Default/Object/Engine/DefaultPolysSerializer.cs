using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultPolysSerializer : BaseObjectSerializer<UPolys>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;
    private readonly IObjectSerializer<FPoly> _polySerializer;

    public DefaultPolysSerializer(IObjectSerializer<UObject> objectSerializer, IObjectSerializer<FPoly> polySerializer)
    {
        _objectSerializer = objectSerializer;
        _polySerializer = polySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UPolys obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        var num = objectStream.ReadInt32();
        var max = objectStream.ReadInt32();
        obj.Element.Super = objectStream.ReadObject();
        _polySerializer.ReadTArrayToList(objectStream, obj.Element.Data, num);
    }

    /// <inheritdoc />
    public override void SerializeObject(UPolys obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteInt32(obj.Element.Data.Count);
        objectStream.WriteInt32(obj.Element.Data.Count);
        objectStream.WriteObject(obj.Element.Super);
        foreach (var fPoly in obj.Element.Data)
        {
            _polySerializer.SerializeObject(fPoly, objectStream);
        }
    }
}

public class DefaultFPolySerializer : BaseObjectSerializer<FPoly>
{
    private readonly IStreamSerializer<FLightmassPrimitiveSettings> _lightmassPrimitiveSettingsSerializer;

    private readonly IStreamSerializer<FVector> _vectorSerializer;

    public DefaultFPolySerializer(IStreamSerializer<FLightmassPrimitiveSettings> lightmassPrimitiveSettingsSerializer,
        IStreamSerializer<FVector> vectorSerializer)
    {
        _lightmassPrimitiveSettingsSerializer = lightmassPrimitiveSettingsSerializer;
        _vectorSerializer = vectorSerializer;
    }


    /// <inheritdoc />
    public override void DeserializeObject(FPoly obj, IUnrealPackageStream objectStream)
    {
        var stream = objectStream.BaseStream;
        obj.Base = _vectorSerializer.Deserialize(stream);
        obj.Normal = _vectorSerializer.Deserialize(stream);
        obj.TextureU = _vectorSerializer.Deserialize(stream);
        obj.TextureV = _vectorSerializer.Deserialize(stream);
        obj.Vertices = _vectorSerializer.ReadTArrayToList(stream);
        obj.PolyFlags = objectStream.ReadUInt32();
        obj.Actor = objectStream.ReadObject();
        obj.ItemName = objectStream.ReadFNameStr();
        obj.Material = objectStream.ReadObject() as UMaterialInterface;
        obj.iLink = objectStream.ReadInt32();
        obj.iBrushPoly = objectStream.ReadInt32();
        obj.ShadowMapScale = objectStream.ReadSingle();
        obj.LightingChannels = objectStream.ReadInt32();
        obj.LightmassSettings = _lightmassPrimitiveSettingsSerializer.Deserialize(stream);
        obj.RulesetVariation = objectStream.ReadFNameStr();
    }

    /// <inheritdoc />
    public override void SerializeObject(FPoly obj, IUnrealPackageStream objectStream)
    {
        var stream = objectStream.BaseStream;
        _vectorSerializer.Serialize(stream, obj.Base);
        _vectorSerializer.Serialize(stream, obj.Normal);
        _vectorSerializer.Serialize(stream, obj.TextureU);
        _vectorSerializer.Serialize(stream, obj.TextureV);
        objectStream.WriteTArray(obj.Vertices, _vectorSerializer);
        objectStream.WriteUInt32(obj.PolyFlags);
        objectStream.WriteObject(obj.Actor);
        objectStream.WriteFName(obj.ItemName);
        objectStream.WriteObject(obj.Material);
        objectStream.WriteInt32(obj.iLink);
        objectStream.WriteInt32(obj.iBrushPoly);
        objectStream.WriteSingle(obj.ShadowMapScale);
        objectStream.WriteInt32(obj.LightingChannels);
        _lightmassPrimitiveSettingsSerializer.Serialize(stream, obj.LightmassSettings);
        objectStream.WriteFName(obj.RulesetVariation);
    }
}