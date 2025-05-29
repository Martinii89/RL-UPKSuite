using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultMaterialSerializer : BaseObjectSerializer<UMaterial>
{
    private readonly IObjectSerializer<FMaterialResource> _materialResourceSerializer;

    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultMaterialSerializer(IObjectSerializer<UObject> objectSerializer, IObjectSerializer<FMaterialResource> materialResourceSerializer)
    {
        _objectSerializer = objectSerializer;
        _materialResourceSerializer = materialResourceSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UMaterial obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);
        obj.ResourceCountFlag = objectStream.ReadInt32();
        obj.FMaterialResources = obj.ResourceCountFlag switch
        {
            1 => _materialResourceSerializer.ReadTArrayToList(objectStream, 1),
            3 => _materialResourceSerializer.ReadTArrayToList(objectStream, 2),
            _ => throw new InvalidDataException()
        };
    }

    /// <inheritdoc />
    public override void SerializeObject(UMaterial obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteInt32(obj.ResourceCountFlag);
        foreach (var resource in obj.FMaterialResources)
        {
            _materialResourceSerializer.SerializeObject(resource, objectStream);
        }
    }
}

public class FmaterialResourceSerializer : BaseObjectSerializer<FMaterialResource>
{
    private readonly IStreamSerializer<FGuid> _fguidSerializer;

    public FmaterialResourceSerializer(IStreamSerializer<FGuid> fguidSerializer)
    {
        _fguidSerializer = fguidSerializer;
    }

    internal int ReadInt(IUnrealPackageStream stream)
    {
        return stream.ReadInt32();
    }

    /// <inheritdoc />
    public override void DeserializeObject(FMaterialResource obj, IUnrealPackageStream objectStream)
    {
        obj.CompileErrors = objectStream.ReadTArray(stream => stream.ReadFString());
        obj.TextureDependencyLengthMap = objectStream.ReadDictionary(stream => stream.ReadObject() as UMaterialExpression, ReadInt);
        obj.MaxTextureDependencyLength = objectStream.ReadInt32();
        obj.ID = _fguidSerializer.Deserialize(objectStream.BaseStream);
        obj.NumUserTexCoords = objectStream.ReadUInt32();
        obj.UniformExpressionTextures = objectStream.ReadTArray(stream => stream.ReadObject() as UTexture);
        obj.bUsesSceneColorTemp = objectStream.ReadBool();
        obj.bUsesSceneDepthTemp = objectStream.ReadBool();
        obj.bUsesDynamicParameterTemp = objectStream.ReadBool();
        obj.bUsesLightmapUVsTemp = objectStream.ReadBool();
        obj.bUsesMaterialVertexPositionOffsetTemp = objectStream.ReadBool();
        obj.UsingTransforms = objectStream.ReadBool();
        obj.FTextureLookupInfos = objectStream.ReadTArray(objectStream1 => new FTextureLookupInfo
        {
            TexCoordIndex = objectStream1.ReadInt32(),
            TextureIndex = objectStream1.ReadInt32(),
            UScale = objectStream1.ReadSingle(),
            VScale = objectStream1.ReadSingle()
        });

        obj.Unk = objectStream.ReadInt32();
        obj.Unk1 = objectStream.ReadInt32();
        obj.Unk2 = objectStream.ReadInt32();
        obj.Unk3 = objectStream.ReadInt32();
    }

    /// <inheritdoc />
    public override void SerializeObject(FMaterialResource obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteTArray(obj.CompileErrors, (stream, val) => stream.WriteFString(val));
        objectStream.WriteDictionary(obj.TextureDependencyLengthMap, (stream, expression) => stream.WriteObject(expression),
            (stream, i) => stream.WriteInt32(i));
        objectStream.WriteInt32(obj.MaxTextureDependencyLength);
        _fguidSerializer.Serialize(objectStream.BaseStream, obj.ID);
        objectStream.WriteUInt32(obj.NumUserTexCoords);
        objectStream.WriteTArray(obj.UniformExpressionTextures, (stream, texture) => stream.WriteObject(texture));
        objectStream.WriteBool(obj.bUsesSceneColorTemp);
        objectStream.WriteBool(obj.bUsesSceneDepthTemp);
        objectStream.WriteBool(obj.bUsesDynamicParameterTemp);
        objectStream.WriteBool(obj.bUsesLightmapUVsTemp);
        objectStream.WriteBool(obj.bUsesMaterialVertexPositionOffsetTemp);
        objectStream.WriteBool(obj.UsingTransforms);
        objectStream.WriteTArray(obj.FTextureLookupInfos, (stream, info) =>
        {
            stream.WriteInt32(info.TexCoordIndex);
            stream.WriteInt32(info.TextureIndex);
            stream.WriteSingle(info.UScale);
            stream.WriteSingle(info.VScale);
        });
        objectStream.WriteInt32(obj.Unk);
        objectStream.WriteInt32(obj.Unk1);
        objectStream.WriteInt32(obj.Unk2);
        objectStream.WriteInt32(obj.Unk3);
    }
}