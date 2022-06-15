using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.FileSummeryInner;

namespace Core.Serialization.Default;

/// <summary>
///     Serializer for FileSummary
/// </summary>
public class FileSummarySerializer : IStreamSerializer<FileSummary>
{
    private readonly IStreamSerializer<FCompressedChunkInfo> _compressedChunkInfoSerializer;
    private readonly IStreamSerializer<FGenerationInfo> _generationsSerializer;
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<FString> _stringSerializer;
    private readonly IStreamSerializer<FTextureType> _textureAllocationsSerializer;

    /// <summary>
    ///     Construct the serializer with all required sub serializers
    /// </summary>
    /// <param name="guidSerializer"></param>
    /// <param name="compressedChunkInfoSerializer"></param>
    /// <param name="stringSerializer"></param>
    /// <param name="textureAllocationsSerializer"></param>
    /// <param name="generationsSerializer"></param>
    public FileSummarySerializer(IStreamSerializer<FGuid> guidSerializer,
        IStreamSerializer<FCompressedChunkInfo> compressedChunkInfoSerializer, IStreamSerializer<FString> stringSerializer,
        IStreamSerializer<FTextureType> textureAllocationsSerializer, IStreamSerializer<FGenerationInfo> generationsSerializer)
    {
        _guidSerializer = guidSerializer;
        _compressedChunkInfoSerializer = compressedChunkInfoSerializer;
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
        fileSummary.Guid = _guidSerializer.Deserialize(stream);
        _generationsSerializer.ReadTArrayToList(stream, fileSummary.Generations);
        fileSummary.EngineVersion = stream.ReadUInt32();
        fileSummary.CookerVersion = stream.ReadUInt32();
        fileSummary.CompressionFlagsOffset = (int) stream.Position;
        fileSummary.CompressionFlags = (ECompressionFlags) stream.ReadUInt32();
        _compressedChunkInfoSerializer.ReadTArrayToList(stream, fileSummary.CompressedChunkInfos);
        fileSummary.Unknown5 = stream.ReadInt32();
        _stringSerializer.ReadTArrayToList(stream, fileSummary.AdditionalPackagesToCook);
        _textureAllocationsSerializer.ReadTArrayToList(stream, fileSummary.TextureAllocations);

        return fileSummary;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FileSummary value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Returns a FileSummarySerializer with all default sub serializers
    /// </summary>
    /// <returns></returns>
    public static FileSummarySerializer GetDefaultSerializer()
    {
        return new FileSummarySerializer(new FGuidSerializer(), new FCompressedChunkInfoSerializer(),
            new FStringSerializer(), new FTextureAllocationsSerializer(new Int32Serializer()),
            new FGenerationInfoSerializer());
    }
}