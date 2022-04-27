using Core.Compression;
using Core.Extensions;
using Core.Types;
using Core.Types.FileSummeryInner;

namespace Core.Serialization.RocketLeage;

public class FileSummarySerializer : RocketLeagueBase, IStreamSerializerFor<FileSummary>
{
    private readonly IStreamSerializerFor<TArray<FCompressedChunkInfo>> _compressedChunkSerializer;
    private readonly IStreamSerializerFor<TArray<FGenerationInfo>> _generationsSerializer;
    private readonly IStreamSerializerFor<FGuid> _guidSerializerFor;
    private readonly IStreamSerializerFor<TArray<FString>> _stringArraySerializer;
    private readonly IStreamSerializerFor<TArray<FTextureType>> _textureAllocationsSerializer;

    public FileSummarySerializer(IStreamSerializerFor<FGuid> guidSerializerFor,
        IStreamSerializerFor<TArray<FGenerationInfo>> generationsSerializer,
        IStreamSerializerFor<TArray<FCompressedChunkInfo>> compressedChunkSerializer, IStreamSerializerFor<TArray<FString>> stringArraySerializer,
        IStreamSerializerFor<TArray<FTextureType>> textureAllocationsSerializer)
    {
        _guidSerializerFor = guidSerializerFor;
        _generationsSerializer = generationsSerializer;
        _compressedChunkSerializer = compressedChunkSerializer;
        _stringArraySerializer = stringArraySerializer;
        _textureAllocationsSerializer = textureAllocationsSerializer;
    }

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
        fileSummary.Generations = _generationsSerializer.Deserialize(stream);
        fileSummary.EngineVersion = stream.ReadUInt32();
        fileSummary.CookerVersion = stream.ReadUInt32();
        fileSummary.CompressionFlagsOffset = (int) stream.Position;
        fileSummary.CompressionFlags = (ECompressionFlags) stream.ReadUInt32();
        fileSummary.CompressedChunks = _compressedChunkSerializer.Deserialize(stream);
        fileSummary.Unknown5 = stream.ReadInt32();
        fileSummary.AdditionalPackagesToCook = _stringArraySerializer.Deserialize(stream);
        fileSummary.TextureAllocations = _textureAllocationsSerializer.Deserialize(stream);
        fileSummary.GarbageSize = stream.ReadInt32();
        fileSummary.CompressedChunkInfoOffset = stream.ReadInt32();
        fileSummary.LastBlockSize = stream.ReadInt32();

        return fileSummary;
    }

    public void Serialize(Stream stream, FileSummary value)
    {
        throw new NotImplementedException();
    }
}