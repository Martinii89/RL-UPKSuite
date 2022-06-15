using Core.Classes.Engine;
using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class FmaterialResourceSerializer : IStreamSerializer<FMaterialResource>
{
    private readonly IStreamSerializer<FGuid> _fguidSerializer;
    private readonly IStreamSerializer<FName> _fnameSerializer;

    private readonly IStreamSerializer<FString> _fstringSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    public FmaterialResourceSerializer(IStreamSerializer<FString> fstringSerializer, IStreamSerializer<FName> fnameSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer, IStreamSerializer<FGuid> fguidSerializer)
    {
        _fstringSerializer = fstringSerializer;
        _fnameSerializer = fnameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _fguidSerializer = fguidSerializer;
    }

    public FMaterialResource Deserialize(Stream stream)
    {
        var res = new FMaterialResource();


        res.Unk = stream.ReadInt32();
        res.CompileErrors = _fstringSerializer.ReadTArrayToList(stream).Select(x => x.InnerString).ToList();
        res.TextureDependencyLengthMap = stream.ReadDictionary(ReadObjectIndex, ReadInt);
        res.MaxTextureDependencyLength = stream.ReadInt32();
        res.ID = _fguidSerializer.Deserialize(stream);
        res.NumUserTexCoords = stream.ReadUInt32();
        res.UniformExpressionTextures = _objectIndexSerializer.ReadTArrayToList(stream);
        res.bUsesSceneColorTemp = stream.ReadInt32() == 1;
        res.bUsesSceneDepthTemp = stream.ReadInt32() == 1;
        res.bUsesDynamicParameterTemp = stream.ReadInt32() == 1;
        res.bUsesLightmapUVsTemp = stream.ReadInt32() == 1;
        res.bUsesMaterialVertexPositionOffsetTemp = stream.ReadInt32() == 1;
        res.UsingTransforms = stream.ReadInt32() == 1;
        res.FTextureLookupInfos = stream.ReadTarray(stream1 => new FTextureLookupInfo
        {
            TexCoordIndex = stream.ReadInt32(),
            TextureIndex = stream.ReadInt32(),
            UScale = stream.ReadSingle(),
            VScale = stream.ReadSingle()
        });

        //res.UnknownBytes = stream.ReadBytes(16);


        return res;
    }

    public void Serialize(Stream stream, FMaterialResource value)
    {
        throw new NotImplementedException();
    }

    internal ObjectIndex ReadObjectIndex(Stream stream)
    {
        return _objectIndexSerializer.Deserialize(stream);
    }

    internal FName ReadFName(Stream stream)
    {
        return _fnameSerializer.Deserialize(stream);
    }

    internal int ReadInt(Stream stream)
    {
        return stream.ReadInt32();
    }
}