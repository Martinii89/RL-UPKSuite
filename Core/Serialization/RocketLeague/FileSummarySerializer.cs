using Core.Extensions;
using Core.Types;
using Core.Types.FileSummeryInner;

namespace Core.Serialization.RocketLeague;

/// <summary>
///     Serializer for FileSummary specifically for cooked RocketLeague packages
/// </summary>
public class FileSummarySerializer : RocketLeagueBase, IStreamSerializerFor<FileSummary>
{
    private readonly IStreamSerializerFor<FCompressedChunkInfo> _compressedChunkSerializer;
    private readonly IStreamSerializerFor<FGenerationInfo> _generationsSerializer;
    private readonly IStreamSerializerFor<FGuid> _guidSerializerFor;
    private readonly IStreamSerializerFor<FString> _stringSerializer;
    private readonly IStreamSerializerFor<FTextureType> _textureAllocationsSerializer;

    /// <summary>
    ///     Construct the serializer with all required sub serializers
    /// </summary>
    /// <param name="guidSerializerFor"></param>
    /// <param name="compressedChunkSerializer"></param>
    /// <param name="stringSerializer"></param>
    /// <param name="textureAllocationsSerializer"></param>
    /// <param name="generationsSerializer"></param>
    public FileSummarySerializer(IStreamSerializerFor<FGuid> guidSerializerFor,
        IStreamSerializerFor<FCompressedChunkInfo> compressedChunkSerializer, IStreamSerializerFor<FString> stringSerializer,
        IStreamSerializerFor<FTextureType> textureAllocationsSerializer, IStreamSerializerFor<FGenerationInfo> generationsSerializer)
    {
        _guidSerializerFor = guidSerializerFor;
        _compressedChunkSerializer = compressedChunkSerializer;
        _stringSerializer = stringSerializer;
        _textureAllocationsSerializer = textureAllocationsSerializer;
        _generationsSerializer = generationsSerializer;
    }

    /// <inheritdoc />
    public FileSummary Deserialize(Stream stream)
    {
        using (stream.TemporarySeek(stream.Position, SeekOrigin.Begin))
        {
            var tag = stream.ReadUInt32();
            if (tag != FileSummary.PackageFileTag)
            {
                throw new Exception("Not a valid Unreal Engine package");
            }
        }

        var fileSummary = new FileSummary();
        fileSummary.Tag = stream.ReadUInt32();
        fileSummary.FileVersion = stream.ReadUInt16();
        fileSummary.LicenseeVersion = stream.ReadUInt16();
        fileSummary.TotalHeaderSize = stream.ReadInt32();
        fileSummary.FolderName = stream.ReadFString();
        fileSummary.PackageFlags = stream.ReadUInt32();
        fileSummary.NameCount = stream.ReadInt32();
        fileSummary.NameOffset = stream.ReadInt32();
        fileSummary.ExportCount = stream.ReadInt32();
        fileSummary.ExportOffset = stream.ReadInt32();
        fileSummary.ImportCount = stream.ReadInt32();
        fileSummary.ImportOffset = stream.ReadInt32();
        fileSummary.DependsOffset = stream.ReadInt32();
        fileSummary.ImportExportGuidsOffset = stream.ReadInt32();
        fileSummary.ImportGuidsCount = stream.ReadInt32();
        fileSummary.ExportGuidsCount = stream.ReadInt32();
        fileSummary.ThumbnailTableOffset = stream.ReadInt32();
        fileSummary.Guid = _guidSerializerFor.Deserialize(stream);
        fileSummary.Generations.AddRange(_generationsSerializer.ReadTArray(stream));
        fileSummary.EngineVersion = stream.ReadUInt32();
        fileSummary.CookerVersion = stream.ReadUInt32();
        fileSummary.CompressionFlagsOffset = (int) stream.Position;
        fileSummary.CompressionFlags = (ECompressionFlags) stream.ReadUInt32();
        fileSummary.CompressedChunks.AddRange(_compressedChunkSerializer.ReadTArray(stream));
        fileSummary.Unknown5 = stream.ReadInt32();
        fileSummary.AdditionalPackagesToCook.AddRange(_stringSerializer.ReadTArray(stream));
        fileSummary.TextureAllocations.AddRange(_textureAllocationsSerializer.ReadTArray(stream));

        return fileSummary;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FileSummary value)
    {
        throw new NotImplementedException();
    }
}