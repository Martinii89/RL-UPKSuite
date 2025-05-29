using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.FileSummeryInner;

namespace Core.Serialization.Default;

/// <summary>
///     Serializer for FileSummary
/// </summary>
public class FileSummarySerializer : IStreamSerializer<FileSummary>
{
    private readonly IStreamSerializer<FCompressedChunk> _compressedChunkSerializer;
    private readonly IStreamSerializer<FGenerationInfo> _generationsSerializer;
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FString> _stringSerializer;
    private readonly IStreamSerializer<FTextureType> _textureAllocationsSerializer;

    /// <summary>
    ///     Construct the serializer with all required sub serializers
    /// </summary>
    /// <param name="guidSerializer"></param>
    /// <param name="compressedChunkSerializer"></param>
    /// <param name="stringSerializer"></param>
    /// <param name="textureAllocationsSerializer"></param>
    /// <param name="generationsSerializer"></param>
    public FileSummarySerializer(IStreamSerializer<FGuid> guidSerializer,
        IStreamSerializer<FCompressedChunk> compressedChunkSerializer, IStreamSerializer<FString> stringSerializer,
        IStreamSerializer<FTextureType> textureAllocationsSerializer, IStreamSerializer<FGenerationInfo> generationsSerializer)
    {
        _guidSerializer = guidSerializer;
        _compressedChunkSerializer = compressedChunkSerializer;
        _stringSerializer = stringSerializer;
        _textureAllocationsSerializer = textureAllocationsSerializer;
        _generationsSerializer = generationsSerializer;
    }

    /// <inheritdoc />
    public FileSummary Deserialize(Stream stream)
    {
        var fileSummary = new FileSummary();
        fileSummary.Tag = stream.ReadUInt32();
        if (fileSummary.Tag != FileSummary.PackageFileTag)
        {
            throw new Exception("Not a valid Unreal Engine package");
        }

        fileSummary.FileVersion = stream.ReadUInt16();
        fileSummary.LicenseeVersion = stream.ReadUInt16();
        fileSummary.TotalHeaderSize = stream.ReadInt32();
        fileSummary.FolderName = stream.ReadFString();
        fileSummary.PackageFlagsFlagsOffset = (int) stream.Position;
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
        fileSummary.Guid = _guidSerializer.Deserialize(stream);
        _generationsSerializer.ReadTArrayToList(stream, fileSummary.Generations);
        fileSummary.EngineVersion = stream.ReadUInt32();
        fileSummary.CookerVersion = stream.ReadUInt32();
        fileSummary.CompressionFlagsOffset = (int) stream.Position;
        fileSummary.CompressionFlags = (ECompressionFlags) stream.ReadUInt32();
        _compressedChunkSerializer.ReadTArrayToList(stream, fileSummary.CompressedChunks);
        fileSummary.Unknown5 = stream.ReadInt32();
        _stringSerializer.ReadTArrayToList(stream, fileSummary.AdditionalPackagesToCook);
        _textureAllocationsSerializer.ReadTArrayToList(stream, fileSummary.TextureAllocations);

        return fileSummary;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FileSummary value)
    {
        stream.WriteUInt32(value.Tag);
        stream.WriteUInt16(value.FileVersion);
        stream.WriteUInt16(value.LicenseeVersion);
        stream.WriteInt32(value.TotalHeaderSize);
        stream.WriteFString(value.FolderName);
        stream.WriteUInt32(value.PackageFlags);
        stream.WriteInt32(value.NameCount);
        stream.WriteInt32(value.NameOffset);
        stream.WriteInt32(value.ExportCount);
        stream.WriteInt32(value.ExportOffset);
        stream.WriteInt32(value.ImportCount);
        stream.WriteInt32(value.ImportOffset);
        stream.WriteInt32(value.DependsOffset);
        stream.WriteInt32(value.ImportExportGuidsOffset);
        stream.WriteInt32(value.ImportGuidsCount);
        stream.WriteInt32(value.ExportGuidsCount);
        stream.WriteInt32(value.ThumbnailTableOffset);
        _guidSerializer.Serialize(stream, value.Guid);
        _generationsSerializer.WriteTArray(stream, value.Generations.ToArray());
        stream.WriteUInt32(value.EngineVersion);
        stream.WriteUInt32(value.CookerVersion);
        stream.WriteUInt32((uint) value.CompressionFlags);
        _compressedChunkSerializer.WriteTArray(stream, value.CompressedChunks.ToArray());
        stream.WriteInt32(value.Unknown5);
        _stringSerializer.WriteTArray(stream, value.AdditionalPackagesToCook.ToArray());
        _textureAllocationsSerializer.WriteTArray(stream, value.TextureAllocations.ToArray());
    }

    /// <summary>
    ///     Returns a FileSummarySerializer with all default sub serializers
    /// </summary>
    /// <returns></returns>
    public static FileSummarySerializer GetDefaultSerializer()
    {
        return new FileSummarySerializer(new FGuidSerializer(), new FCompressedChunkSerializer(),
            new FStringSerializer(), new FTextureAllocationsSerializer(),
            new FGenerationInfoSerializer());
    }
}