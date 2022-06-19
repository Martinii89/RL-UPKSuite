using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class FmaterialResourceSerializer : BaseObjectSerializer<FMaterialResource>
{
    private readonly IStreamSerializer<FGuid> _fguidSerializer;
    private readonly IStreamSerializer<FString> _fstringSerializer;

    public FmaterialResourceSerializer(IStreamSerializer<FString> fstringSerializer, IStreamSerializer<FGuid> fguidSerializer)
    {
        _fstringSerializer = fstringSerializer;
        _fguidSerializer = fguidSerializer;
    }

    internal int ReadInt(IUnrealPackageStream stream)
    {
        return stream.ReadInt32();
    }

    public override void DeserializeObject(FMaterialResource obj, IUnrealPackageStream objectStream)
    {
        obj.Unk = objectStream.ReadInt32();
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
    }

    public override void SerializeObject(FMaterialResource obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}