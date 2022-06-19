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
        obj.FMaterialResources[0].UnknownBytes = objectStream.ReadBytes((int) leftOver);
    }

    public override void SerializeObject(UMaterial obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}

public class FmaterialResourceSerializer : BaseObjectSerializer<FMaterialResource>
{
    private readonly IStreamSerializer<FGuid> _fguidSerializer;
    private readonly IStreamSerializer<FString> _fstringSerializer;

    public FmaterialResourceSerializer(IStreamSerializer<FString> fstringSerializer, IStreamSerializer<FGuid> fguidSerializer)
    {
        _fstringSerializer = fstringSerializer;
        _fguidSerializer = fguidSerializer;
    }


    internal FName ReadFName(IUnrealPackageStream stream)
    {
        return stream.ReadFName();
    }

    internal int ReadInt(IUnrealPackageStream stream)
    {
        return stream.ReadInt32();
    }

    public override void DeserializeObject(FMaterialResource obj, IUnrealPackageStream objectStream)
    {
        obj.CompileErrors = _fstringSerializer.ReadTArrayToList(objectStream.BaseStream).Select(x => x.InnerString).ToList();
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

        obj.UnknownBytes = objectStream.ReadBytes(16);
    }

    public override void SerializeObject(FMaterialResource obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}