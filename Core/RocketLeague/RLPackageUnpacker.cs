using System.Buffers;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;

using RlUpk.Core.Flags;
using RlUpk.Core.RocketLeague.Decryption;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Default;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types;
using RlUpk.Core.Types.FileSummeryInner;

using FCompressedChunkSerializer = RlUpk.Core.Serialization.RocketLeague.FCompressedChunkSerializer;

namespace RlUpk.Core.RocketLeague;

/// <summary>
///     Unpacks a Rocket League encrypted and compressed UnrealPackage.
/// </summary>
public class RLPackageUnpacker
{
    private readonly FCompressedChunkinfoSerializer _blockSerializer = new();
    private readonly FCompressedChunkHeaderSerializer _compressedChunkHeaderSerializer;
    private readonly FCompressedChunkSerializer _compressedChunkSerializer = new();
    private readonly IDecrypterProvider _decrypterProvider;
    private readonly Stream _inputStream;

    /// <summary>
    ///     Starts unpacking the data from the input stream. It does little to no checks to verify it is a RocketLeague
    ///     package. Check the Deserialization state or the Valid property to know if unpacking was successful
    /// </summary>
    /// <param name="inputStream">The package content stream</param>
    /// <param name="decrypterProvider">Required to unpack the decrypted data</param>
    /// <param name="fileSummarySerializer">Serializer for the header data</param>
    public RLPackageUnpacker(Stream inputStream, IDecrypterProvider decrypterProvider,
        IStreamSerializer<FileSummary> fileSummarySerializer)
    {
        _inputStream = inputStream;
        _decrypterProvider = decrypterProvider;
        // TODO: use DI
        _compressedChunkHeaderSerializer = new FCompressedChunkHeaderSerializer(_blockSerializer);
        FileSummary = fileSummarySerializer.Deserialize(inputStream);
    }

    /// <summary>
    ///     The Parsed FileSummary of this package
    /// </summary>
    public FileSummary FileSummary { get; }

    private FileCompressionMetaData FileCompressionMetaData { get; set; } = new();

    /// <summary>
    ///     The state of unpacking. Anything but UnpackResult.Inflated would indicate some kind of error.
    /// </summary>
    public UnpackResult UnpackResult { get; private set; } = UnpackResult.None;

    private int EncryptedSize
    {
        get
        {
            var encryptedSize = FileSummary.TotalHeaderSize - FileCompressionMetaData.GarbageSize - FileSummary.NameOffset;
            encryptedSize = (encryptedSize + 15) & ~15; // Round up to the next block
            return encryptedSize;
        }
    }

    /// <summary>
    ///     Returns true if we successfully unpacked the package
    /// </summary>
    public bool Valid => UnpackResult.HasFlag(UnpackResult.Inflated);

    /// <summary>
    ///     Unpack the package and returns a stream to the unpacked package data.
    /// </summary>
    /// <returns></returns>
    public UnpackResult Unpack(Stream outputStream)
    {
        ProcessFileSummary(outputStream, _inputStream);

        ProcessDecryptedData(outputStream, _inputStream, _decrypterProvider);
        if (!UnpackResult.HasFlag(UnpackResult.Decrypted))
        {
            return UnpackResult;
        }

        ProcessCompressedData(outputStream, _inputStream);
        return UnpackResult;
    }

    private void ProcessCompressedData(Stream outputStream, Stream inputBinaryReader)
    {
        // var uncompressedDataBuffer = new byte[FileSummary.CompressedChunks.Sum(info => info.UncompressedSize)];
        // var uncompressOutputStream = new MemoryStream(uncompressedDataBuffer);

        var firstUncompressedOffset = FileSummary.CompressedChunks.First().UncompressedOffset;
        var finalSize = FileSummary.CompressedChunks.Last().UncompressedOffset + FileSummary.CompressedChunks.Last().UncompressedSize;
        outputStream.SetLength(finalSize);
        outputStream.Position = firstUncompressedOffset;

        var uncompressProgress = 0L;
        // Decompress compressed chunks
        foreach (var chunk in FileSummary.CompressedChunks)
        {
            inputBinaryReader.Position = chunk.CompressedOffset;
            var chunkHeader = _compressedChunkHeaderSerializer.Deserialize(inputBinaryReader);

            var sumUncompressedSize = 0;
            var blocks = new List<FCompressedChunkInfo>();

            while (sumUncompressedSize < chunkHeader.Summary.UncompressedSize)
            {
                var block = _blockSerializer.Deserialize(inputBinaryReader);
                blocks.Add(block);
                sumUncompressedSize += block.UncompressedSize;
            }

            var largestBlock = blocks.Max(x => x.CompressedSize);
            var buffer = ArrayPool<byte>.Shared.Rent(largestBlock);

            foreach (var block in blocks)
            {
                var blockSlize = buffer.AsSpan()[..block.CompressedSize];
                inputBinaryReader.ReadExactly(blockSlize);
                using var zlibStream = new ZLibStream(new MemoryStream(buffer, 0, block.CompressedSize), CompressionMode.Decompress);
                zlibStream.CopyTo(outputStream);
                
            }
            ArrayPool<byte>.Shared.Return(buffer);

            // Debug.Assert(uncompressOutputStream.Position == uncompressProgress + chunk.UncompressedSize);
            uncompressProgress += chunk.UncompressedSize;
        }

        UnpackResult |= UnpackResult.Inflated;

        // Debug.Assert(uncompressOutputStream.Position == uncompressOutputStream.Length);
        // outputStream.Write(uncompressedDataBuffer);


        // Reset the Cooked package flag so UModel won't try to decrypt it again
        outputStream.Position = FileSummary.PackageFlagsFlagsOffset;
        var notCooked = ((PackageFlags)FileSummary.PackageFlags) & ~PackageFlags.PKG_Cooked;
        outputStream.Write((uint)notCooked);
        
        // Reset the compression flag to indicate this package is no longer compressed.
        outputStream.Position = FileSummary.CompressionFlagsOffset;
        outputStream.Write((int) ECompressionFlags.CompressNone);
    }

    private void ProcessDecryptedData(Stream outputStream, Stream inputStream, IDecrypterProvider decrypterProvider)
    {
        byte[] decryptedData;
        try
        {
            decryptedData = DecryptData(inputStream, decrypterProvider);
        }
        catch (InvalidDataException)
        {
            return;
        }

        var decryptedDataReader = new MemoryStream(decryptedData);

        decryptedDataReader.Position = FileCompressionMetaData.CompressedChunksffset;

        _compressedChunkSerializer.ReadTArrayToList(decryptedDataReader, FileSummary.CompressedChunks);
        // The depends table is always empty. So The depends table marks the start of where the uncompressed data should go.
        Debug.Assert(FileSummary.CompressedChunks.First().UncompressedOffset == FileSummary.DependsOffset);
        outputStream.Write(decryptedData);
        UnpackResult |= UnpackResult.Decrypted;
    }

    private void ProcessFileSummary(Stream outputStream, Stream inputStream)
    {
        FileCompressionMetaData = FileCompressionMetaData.Deserialize(inputStream);
        inputStream.Position = 0;
        var fileSummaryBytes = inputStream.ReadBytes(FileSummary.NameOffset);
        outputStream.Write(fileSummaryBytes);

        UnpackResult |= UnpackResult.Header;
        if (!FileSummary.CompressionFlags.HasFlag(ECompressionFlags.CompressZlib))
        {
            throw new InvalidDataException("Package compression type is unsupported ");
        }
    }

    private byte[] DecryptData(Stream dataStream, IDecrypterProvider decrypterProvider)
    {
        dataStream.Seek(FileSummary.NameOffset, SeekOrigin.Begin);
        var encryptedData = dataStream.ReadBytes(EncryptedSize);
        if (encryptedData.Length != EncryptedSize)
        {
            throw new InvalidDataException("Failed to read the encrypted data from the stream");
        }

        var validDecryptor = decrypterProvider.DecryptionKeys.Select(decrypterProvider.GetCryptoTransform)
            .FirstOrDefault(x => VerifyDecryptor(x, encryptedData));
        if (validDecryptor == null)
        {
            Console.WriteLine("Unknown Decryption key");
            throw new InvalidDataException("Unknown Decryption key");
        }
        return validDecryptor.TransformFinalBlock(encryptedData, 0, EncryptedSize);
    }

    /// <summary>
    ///     Decrypts the part containing the CompressedChunk info. Verifies that it contains logical and valid data once
    ///     decrypted
    /// </summary>
    /// <param name="decryptor"></param>
    /// <param name="encryptedData"></param>
    /// <returns></returns>
    private bool VerifyDecryptor(ICryptoTransform decryptor, byte[] encryptedData)
    {
        var blockOffset = FileCompressionMetaData.CompressedChunksffset % 16;
        var blockStart = FileCompressionMetaData.CompressedChunksffset - blockOffset;
        var chunkInfoBytes = new byte[32];

        decryptor.TransformBlock(encryptedData, blockStart, 32, chunkInfoBytes, 0);

        var binaryReader = new BinaryReader(new MemoryStream(new Span<byte>(chunkInfoBytes)[blockOffset..].ToArray()));
        var chunkInfoLength = binaryReader.ReadInt32();
        var uncompressedOffsetFirstChunk = binaryReader.ReadInt32();
        return chunkInfoLength >= 1 && uncompressedOffsetFirstChunk == FileSummary.DependsOffset;
    }
}