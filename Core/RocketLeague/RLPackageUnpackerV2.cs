using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;

using RlUpk.Core.Flags;
using RlUpk.Core.IO;
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
public class RLPackageUnpackerV2
{
    private readonly FCompressedChunkinfoSerializer _blockSerializer = new();
    private readonly FCompressedChunkHeaderSerializer _compressedChunkHeaderSerializer;
    private readonly FCompressedChunkSerializer _compressedChunkSerializer = new();
    private readonly IDecrypterProvider _decrypterProvider;
    private readonly IStreamSerializer<FileSummary> _fileSummarySerializer;

    /// <summary>
    ///     Starts unpacking the data from the input stream. It does little to no checks to verify it is a RocketLeague
    ///     package. Check the Deserialization state or the Valid property to know if unpacking was successful
    /// </summary>
    /// <param name="decrypterProvider">Required to unpack the decrypted data</param>
    /// <param name="fileSummarySerializer">Serializer for the header data</param>
    public RLPackageUnpackerV2(IDecrypterProvider decrypterProvider,
        IStreamSerializer<FileSummary> fileSummarySerializer)
    {
        _decrypterProvider = decrypterProvider;
        _fileSummarySerializer = fileSummarySerializer;
        // TODO: use DI
        _compressedChunkHeaderSerializer = new FCompressedChunkHeaderSerializer(_blockSerializer);
    }

    // /// <summary>
    // ///     The Parsed FileSummary of this package
    // /// </summary>
    // private FileSummary FileSummary { get; }

    private FileCompressionMetaData FileCompressionMetaData { get; set; } = new();

    // /// <summary>
    // ///     The state of unpacking. Anything but UnpackResult.Inflated would indicate some kind of error.
    // /// </summary>
    // public UnpackResult UnpackResult { get; private set; } = UnpackResult.None;
    //
    // /// <summary>
    // ///     Returns true if we successfully unpacked the package
    // /// </summary>
    // public bool Valid => UnpackResult.HasFlag(UnpackResult.Inflated);

    private int GetEncryptedSize(FileSummary fileSummary)
    {
        int encryptedSize = fileSummary.TotalHeaderSize - FileCompressionMetaData.GarbageSize - fileSummary.NameOffset;
        encryptedSize = (encryptedSize + 15) & ~15; // Round up to the next block
        return encryptedSize;
    }

    /// <summary>
    ///     Unpack the package and returns a stream to the unpacked package data.
    /// </summary>
    /// <returns></returns>
    public UnpackResult Unpack(Stream inputStream, Stream outputStream)
    {
        var result = UnpackResult.None;
        var fileSummary = _fileSummarySerializer.Deserialize(inputStream);
        if (!ProcessFileSummary(fileSummary, outputStream, inputStream))
        {
            return result;
        }

        result |= UnpackResult.Header;

        if (!ProcessDecryptedData(fileSummary, outputStream, inputStream))
        {
            return result;
        }

        result |= UnpackResult.Decrypted;

        if (!ProcessCompressedData(fileSummary, outputStream, inputStream))
        {
            return result;
        }

        result |= UnpackResult.Inflated;
        return result;
    }

    private bool ProcessCompressedData(FileSummary fileSummary, Stream outputStream, Stream inputBinaryReader)
    {
        long firstUncompressedOffset = fileSummary.CompressedChunks.First().UncompressedOffset;
        var finalSize = fileSummary.CompressedChunks.Last().UncompressedOffset + fileSummary.CompressedChunks.Last().UncompressedSize;
        outputStream.SetLength(finalSize);
        outputStream.Position = firstUncompressedOffset;

        // Decompress compressed chunks
        foreach (FCompressedChunk chunk in fileSummary.CompressedChunks)
        {
            if (inputBinaryReader.Position != chunk.CompressedOffset)
            {
                inputBinaryReader.Position = chunk.CompressedOffset;
            }
            FCompressedChunkHeader chunkHeader = _compressedChunkHeaderSerializer.Deserialize(inputBinaryReader);

            var blocks = _blockSerializer.ReadTArrayToList(inputBinaryReader, chunkHeader.BlockCount);

            foreach (FCompressedChunkInfo block in blocks)
            {
                var blockStreamSlice = new StreamSlice(inputBinaryReader, block.CompressedSize);
                using ZLibStream zlibStream = new(blockStreamSlice, CompressionMode.Decompress);
                zlibStream.CopyTo(outputStream);
            }
        }
        
        // Reset the Cooked package flag so UModel won't try to decrypt it again
        outputStream.Position = fileSummary.PackageFlagsFlagsOffset;
        var notCooked = ((PackageFlags)fileSummary.PackageFlags) & ~PackageFlags.PKG_Cooked;
        outputStream.Write((uint)notCooked);

        // Reset the compression flag to indicate this package is no longer compressed.
        outputStream.Position = fileSummary.CompressionFlagsOffset;
        outputStream.Write((int)ECompressionFlags.CompressNone);
        return true;
    }

    private bool ProcessDecryptedData(FileSummary fileSummary, Stream outputStream, Stream inputStream)
    {
        var decryptResult = GetDecryptedStream2(fileSummary, inputStream);
        if (decryptResult.stream is null || decryptResult.decryptError is not DecryptError.None)
        {
            return false;
        }
        using var decryptStream = decryptResult.stream;
        
        // The depends table is always empty. So The depends table marks the start of where the uncompressed data should go.
        var chunksOffset = outputStream.Position + FileCompressionMetaData.CompressedChunksffset;
        var p = outputStream.Position;
        // if (outputStream is MemoryStream memoryStream)
        // {
        //     memoryStream.SetLength(memoryStream.Position + GetEncryptedSize(fileSummary));
        // }
        outputStream.SetLength(outputStream.Position + GetEncryptedSize(fileSummary));
        decryptStream.CopyTo(outputStream);
        var l = outputStream.Position - p;
        
        using var tempSeek = outputStream.TemporarySeek(chunksOffset, SeekOrigin.Begin);
        _compressedChunkSerializer.ReadTArrayToList(outputStream, fileSummary.CompressedChunks);
        Debug.WriteLine($"NameOffset: {fileSummary.NameOffset}\nEncryptedSize: {GetEncryptedSize(fileSummary)}\nFirstCompressedChunk: {fileSummary.CompressedChunks.First().CompressedOffset}\nInputPosition: {inputStream.Position}");
        Debug.Assert(fileSummary.CompressedChunks.First().UncompressedOffset == fileSummary.DependsOffset);
        return true;
    }

    private bool ProcessFileSummary(FileSummary fileSummary, Stream outputStream, Stream inputStream)
    {
        FileCompressionMetaData = FileCompressionMetaData.Deserialize(inputStream);
        inputStream.Position = 0;
        var fileSummaryBytes = inputStream.ReadBytes(fileSummary.NameOffset);
        outputStream.Write(fileSummaryBytes);

        if (!fileSummary.CompressionFlags.HasFlag(ECompressionFlags.CompressZlib))
        {
            throw new InvalidDataException("Package compression type is unsupported ");
        }

        return true;
    }

    private enum DecryptError
    {
        None, BadRead, MissingKey
    }
    
    private (Stream? stream, DecryptError decryptError) GetDecryptedStream(FileSummary fileSummary, Stream dataStream)
    {
        dataStream.Seek(fileSummary.NameOffset, SeekOrigin.Begin);
        int encryptedSize = GetEncryptedSize(fileSummary);
        var encryptedSlice = new StreamSlice(dataStream, encryptedSize);
        
        byte[] blockData = new byte[32];
        ReadFirstBlockData(encryptedSlice, blockData);

        var (transform, validDecryptor) = _decrypterProvider.DecryptionKeys.Select(_decrypterProvider.GetAes)
            .FirstOrDefault(x => VerifyDecryptor(x.aes, fileSummary, blockData));
        if (validDecryptor == null)
        {
            Console.WriteLine("Unknown Decryption key");
            return (null, DecryptError.MissingKey);
            // throw new InvalidDataException("Unknown Decryption key");
        }
        
        return (new CryptoStream(encryptedSlice, transform, CryptoStreamMode.Read), DecryptError.None);
    }
    
    private (Stream? stream, DecryptError decryptError) GetDecryptedStream2(FileSummary fileSummary, Stream dataStream)
    {
        dataStream.Seek(fileSummary.NameOffset, SeekOrigin.Begin);
        var encryptedSize = GetEncryptedSize(fileSummary);
        var encryptedBuffer = ArrayPool<byte>.Shared.Rent(encryptedSize);
        dataStream.ReadExactly(encryptedBuffer, 0, encryptedSize);
        
        byte[] blockData = new byte[32];
        ReadFirstBlockData2(encryptedBuffer, blockData);

        var (transform, validDecryptor) = _decrypterProvider.DecryptionKeys.Select(_decrypterProvider.GetAes)
            .FirstOrDefault(x => VerifyDecryptor(x.aes, fileSummary, blockData));
        if (validDecryptor == null)
        {
            Console.WriteLine("Unknown Decryption key");
            return (null, DecryptError.MissingKey);
            // throw new InvalidDataException("Unknown Decryption key");
        }

        MemoryStream decryptedStream2 = new MemoryStream(transform.TransformFinalBlock(encryptedBuffer, 0, encryptedSize));
        ArrayPool<byte>.Shared.Return(encryptedBuffer);
        return (decryptedStream2, DecryptError.None);
        // return (new CryptoStream(encryptedSlice, transform, CryptoStreamMode.Read), DecryptError.None);
    }

    private void ReadFirstBlockData(Stream stream, Span<byte> buffer)
    {
        var blockOffset = FileCompressionMetaData.CompressedChunksffset % 16;
        var blockStart = FileCompressionMetaData.CompressedChunksffset - blockOffset;
        using var tempSeek = stream.TemporarySeek(blockStart, SeekOrigin.Begin);
        stream.ReadExactly(buffer);
    }

    private void ReadFirstBlockData2(byte[] stream, Span<byte> buffer)
    {
        var blockOffset = FileCompressionMetaData.CompressedChunksffset % 16;
        var blockStart = FileCompressionMetaData.CompressedChunksffset - blockOffset;
        stream.AsSpan(blockStart, 32).CopyTo(buffer);
    }
    
    /// <summary>
    ///     Decrypts the part containing the CompressedChunk info. Verifies that it contains logical and valid data once
    ///     decrypted
    /// </summary>
    /// <param name="decryptor"></param>
    /// <param name="fileSummary"></param>
    /// <param name="blockData"></param>
    /// <returns></returns>
    private bool VerifyDecryptor(Aes decryptor, FileSummary fileSummary, ReadOnlySpan<byte> blockData)
    {
        int blockOffset = FileCompressionMetaData.CompressedChunksffset % 16;
        Span<byte> buffer = stackalloc byte[32];

        decryptor.DecryptEcb(blockData, buffer, PaddingMode.None);

        var chunkInfoLength = BinaryPrimitives.ReadInt32LittleEndian(buffer[blockOffset..]);
        var uncompressedOffsetFirstChunk = BinaryPrimitives.ReadInt32LittleEndian(buffer[(blockOffset+4)..]);
        return chunkInfoLength >= 1 && uncompressedOffsetFirstChunk == fileSummary.DependsOffset;
    }
    
    private bool VerifyDecryptor2(ICryptoTransform decryptor, FileSummary fileSummary, byte[] blockData)
    {
        int blockOffset = FileCompressionMetaData.CompressedChunksffset % 16;
        var buffer = new byte[32];

        decryptor.TransformBlock(blockData, 0, 32, buffer, 0);

        var chunkInfoLength = BinaryPrimitives.ReadInt32LittleEndian(buffer[blockOffset..]);
        var uncompressedOffsetFirstChunk = BinaryPrimitives.ReadInt32LittleEndian(buffer[(blockOffset+4)..]);
        return chunkInfoLength >= 1 && uncompressedOffsetFirstChunk == fileSummary.DependsOffset;
    }
    
    // private bool VerifyDecryptor(ICryptoTransform decryptor, FileSummary fileSummary, byte[] encryptedData)
    // {
    //     var blockOffset = FileCompressionMetaData.CompressedChunksffset % 16;
    //     var blockStart = FileCompressionMetaData.CompressedChunksffset - blockOffset;
    //     var chunkInfoBytes = new byte[32];
    //
    //     decryptor.TransformBlock(encryptedData, blockStart, 32, chunkInfoBytes, 0);
    //
    //     var binaryReader = new BinaryReader(new MemoryStream(new Span<byte>(chunkInfoBytes)[blockOffset..].ToArray()));
    //     var chunkInfoLength = binaryReader.ReadInt32();
    //     var uncompressedOffsetFirstChunk = binaryReader.ReadInt32();
    //     return chunkInfoLength >= 1 && uncompressedOffsetFirstChunk == fileSummary.DependsOffset;
    // }
}