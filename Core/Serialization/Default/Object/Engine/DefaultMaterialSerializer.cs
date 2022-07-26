using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultMaterialSerializer : BaseObjectSerializer<UMaterial>
{
    private readonly IObjectSerializer<FMaterialResource> _materialResourceSerializer;

    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultMaterialSerializer(IObjectSerializer<UObject> objectSerializer, IObjectSerializer<FMaterialResource> materialResourceSerializer)
    {
        _objectSerializer = objectSerializer;
        _materialResourceSerializer = materialResourceSerializer;
    }

    public override void DeserializeObject(UMaterial obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        obj.FMaterialResources[0] = new FMaterialResource();
        _materialResourceSerializer.DeserializeObject(obj.FMaterialResources[0], objectStream);

        var leftOver = obj.ExportTableItem.SerialOffset + obj.ExportTableItem.SerialSize - objectStream.BaseStream.Position;
        obj.FMaterialResources[0].UnknownBytes2 = objectStream.ReadBytes((int) leftOver);
    }

    public override void SerializeObject(UMaterial obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        _materialResourceSerializer.SerializeObject(obj.FMaterialResources[0], objectStream);
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

    public override void DeserializeObject(FMaterialResource obj, IUnrealPackageStream objectStream)
    {
        obj.CompileErrors = objectStream.ReadTArray(stream => stream.ReadFString());
        obj.TextureDependencyLengthMap = objectStream.ReadDictionary(stream => stream.ReadObject() as UMaterialExpression, ReadInt);
        obj.MaxTextureDependencyLength = objectStream.ReadInt32();
        obj.ID = _fguidSerializer.Deserialize(objectStream.BaseStream);
        obj.NumUserTexCoords = objectStream.ReadUInt32();
        obj.UniformExpressionTextures = objectStream.ReadTArray(stream => stream.ReadObject() as UTexture);
        obj.bUsesSceneColorTemp = objectStream.ReadInt32() == 1;
        obj.bUsesSceneDepthTemp = objectStream.ReadInt32() == 1;
        obj.bUsesDynamicParameterTemp = objectStream.ReadInt32() == 1;
        obj.bUsesLightmapUVsTemp = objectStream.ReadInt32() == 1;
        obj.bUsesMaterialVertexPositionOffsetTemp = objectStream.ReadInt32() == 1;
        obj.UsingTransforms = objectStream.ReadInt32() == 1;
        obj.FTextureLookupInfos = objectStream.ReadTArray(objectStream1 => new FTextureLookupInfo
        {
            TexCoordIndex = objectStream.ReadInt32(),
            TextureIndex = objectStream.ReadInt32(),
            UScale = objectStream.ReadSingle(),
            VScale = objectStream.ReadSingle()
        });

        obj.FourUnknownInts = objectStream.ReadBytes(16);
    }

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
        objectStream.WriteBytes(obj.FourUnknownInts ?? throw new InvalidOperationException());
    }
}