using System.Buffers;
using System.Diagnostics;
using System.IO.Compression;

using RlUpk.Core.Flags;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;
using RlUpk.Core.Types.FileSummeryInner;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Classes.Compression;

public class PackageCompressor
{
    private readonly IStreamSerializer<FCompressedChunkInfo> _compressedChunkInfoSerializer;
    private readonly IStreamSerializer<ExportTableItem> _exportTableItemSerializer;

    private readonly IStreamSerializer<FileSummary> _headerSerializer;
    private List<FCompressedChunk> _chunks = new();
    private FCompressedChunk _currentChunk = new();

    public PackageCompressor(IStreamSerializer<FileSummary> headerSerializer, IStreamSerializer<ExportTableItem> exportTableItemSerializer,
        IStreamSerializer<FCompressedChunkInfo> compressedChunkInfoSerializer)
    {
        _headerSerializer = headerSerializer;
        _exportTableItemSerializer = exportTableItemSerializer;
        _compressedChunkInfoSerializer = compressedChunkInfoSerializer;
    }

    /// <summary>
    /// </summary>
    /// <param name="sourceStream">The stream of the uncompressed package</param>
    /// <param name="outputStream">The stream where the compressed data will be written to</param>
    /// <exception cref="InvalidDataException">If package is already compressed</exception>
    public void CompressFile(Stream sourceStream, Stream outputStream)
    {
        sourceStream.Position = 0;
        var header = _headerSerializer.Deserialize(sourceStream);
        if (((PackageFlags) header.PackageFlags).HasFlag(PackageFlags.PKG_StoreCompressed))
        {
            throw new InvalidDataException("Package already compressed");
        }

        var startOffset = sourceStream.Position;
        var restHeaderSize = (int) (header.TotalHeaderSize - startOffset);

        _currentChunk.UncompressedSize = restHeaderSize;
        _currentChunk.UncompressedOffset = startOffset;

        sourceStream.Position = header.ExportOffset;

        // ReSharper disable once CollectionNeverUpdated.Local
        var exportTable = new ExportTable(_exportTableItemSerializer, sourceStream, header.ExportCount);


        sourceStream.Position = startOffset;

        foreach (var exportItem in exportTable)
        {
            AddToChunk(exportItem.SerialSize);
        }

        FinishCurrentAndCreateNewChunk(0);

        header.CompressionFlags = ECompressionFlags.CompressZlib;
        header.CompressedChunks = _chunks;
        header.PackageFlags |= 0x02000000;
        _headerSerializer.Serialize(outputStream, header);

        _currentChunk = new FCompressedChunk();
        _chunks = new List<FCompressedChunk>();
        foreach (var chunkInfo in header.CompressedChunks)
        {
            if (chunkInfo.UncompressedOffset != sourceStream.Position)
            {
                Debugger.Break();
            }

            chunkInfo.CompressedOffset = outputStream.Position;
            SerializeCompressed(sourceStream, chunkInfo.UncompressedSize, outputStream);
            outputStream.Flush();
            chunkInfo.CompressedSize = (int) (outputStream.Position - chunkInfo.CompressedOffset);
        }

        if (!sourceStream.IsEndOfStream())
        {
            Debugger.Break();
        }

        outputStream.Position = 0;
        _headerSerializer.Serialize(outputStream, header);
        outputStream.Flush();
    }

    private void SerializeCompressed(Stream srcStream, int dataSize, Stream outputStream)
    {
        outputStream.WriteUInt32(0x9E2A83C1);
        const int compressionChunkSize = 0x20000;
        outputStream.WriteInt32(compressionChunkSize);

        var chunkCount = (dataSize + compressionChunkSize - 1) / compressionChunkSize + 1;

        var startOffset = outputStream.Position;
        var chunks = new List<FCompressedChunkInfo>();
        for (var i = 0; i < chunkCount; i++)
        {
            var chunkInfo = new FCompressedChunkInfo();
            chunks.Add(chunkInfo);
            _compressedChunkInfoSerializer.Serialize(outputStream, chunkInfo);
        }

        // summary chunk
        chunks[0].UncompressedSize = dataSize;
        chunks[0].CompressedSize = 0;

        var remaining = dataSize;
        var chunkIndex = 1;
        {
            var buffer = ArrayPool<byte>.Shared.Rent(compressionChunkSize);
            while (remaining > 0)
            {
                var toCompress = Math.Min(compressionChunkSize, remaining);
                
                srcStream.ReadExactly(buffer, 0, toCompress);
                // var uncompressedData = srcStream.ReadBytes(toCompress);
                var uncompressedStream = new MemoryStream(buffer, 0, toCompress);
                var start = outputStream.Position;
                {
                    using var zlibStream = new ZLibStream(outputStream, CompressionMode.Compress, true);
                    uncompressedStream.WriteTo(zlibStream);
                }
                var compressedSize = (int) (outputStream.Position - start);

                chunks[chunkIndex].UncompressedSize = toCompress;
                chunks[chunkIndex].CompressedSize = compressedSize;
                chunks[0].CompressedSize += compressedSize;

                chunkIndex++;
                remaining -= compressionChunkSize;
                
            }
            ArrayPool<byte>.Shared.Return(buffer);

        }


        var endOffset = outputStream.Position;
        outputStream.Position = startOffset;
        for (var i = 0; i < chunkCount; i++)
        {
            _compressedChunkInfoSerializer.Serialize(outputStream, chunks[i]);
        }

        outputStream.Position = endOffset;
    }

    private void FinishCurrentAndCreateNewChunk(int size)
    {
        _chunks.Add(_currentChunk);
        var offset = _currentChunk.UncompressedOffset + _currentChunk.UncompressedSize;
        _currentChunk = new FCompressedChunk
        {
            UncompressedSize = size,
            UncompressedOffset = offset
        };
    }

    private void AddToChunk(int serialSize)
    {
        if (_currentChunk.UncompressedSize + serialSize > 1024 * 1024)
        {
            FinishCurrentAndCreateNewChunk(serialSize);
        }
        else
        {
            _currentChunk.UncompressedSize += serialSize;
        }
    }
}