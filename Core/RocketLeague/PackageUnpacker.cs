using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using Core.Compression;
using Core.RocketLeague.Decryption;
using Core.Types;
using Core.Types.FileSummeryInner;

namespace Core.RocketLeague;

[Flags]
public enum DeserializationState
{
    None = 0,
    Header = 1,
    Decrypted = 2,
    Inflated = 4
}

public class PackageUnpacker
{
    /// <summary>
    /// Starts unpacking the data from the input stream. It does little to no checks to verify it is a RocketLeague package. Check the Deserialization state or the Valid property to know if unpacking was successful
    /// </summary>
    /// <param name="inputStream">The package content stream</param>
    /// <param name="outputStream">Unpacked package will be written to this</param>
    /// <param name="decrypterProvider">Required to unpack the decrypted data</param>
    public PackageUnpacker(Stream inputStream, Stream outputStream, IDecrypterProvider decrypterProvider)
    {
        var inputBinaryReader = new BinaryReader(inputStream);

        ProcessFileSummary(outputStream, inputBinaryReader);

        ProcessDecryptedData(outputStream, inputBinaryReader, decrypterProvider);
        if (!DeserializationState.HasFlag(DeserializationState.Decrypted))
        {
            return;
        }

        ProcessCompressedData(outputStream, inputBinaryReader);
    }

    /// <summary>
    /// The Parsed FileSummary of this package
    /// </summary>
    public FileSummary FileSummary { get; } = new();

    /// <summary>
    /// The state of unpacking. Anything but DeserializationState.Inflated would indicate some kind of error.
    /// </summary>
    public DeserializationState DeserializationState { get; private set; } = DeserializationState.None;

    private int EncryptedSize
    {
        get
        {
            var encryptedSize = FileSummary.TotalHeaderSize - FileSummary.GarbageSize - FileSummary.NameOffset;
            encryptedSize = (encryptedSize + 15) & ~15; // Round up to the next block
            return encryptedSize;
        }
    }

    /// <summary>
    /// Returns true if we successfully unpacked the package
    /// </summary>
    public bool Valid => DeserializationState.HasFlag(DeserializationState.Inflated);

    private void ProcessCompressedData(Stream outputStream, BinaryReader inputBinaryReader)
    {
        var uncompressedDataBuffer = new byte[FileSummary.CompressedChunks.Sum(info => info.UncompressedSize)];
        var uncompressOutputStream = new MemoryStream(uncompressedDataBuffer);
        var firstUncompressedOffset = FileSummary.CompressedChunks.First().UncompressedOffset;
        var uncompressProgress = 0L;
        // Decompress compressed chunks
        foreach (var chunk in FileSummary.CompressedChunks)
        {
            inputBinaryReader.BaseStream.Position = chunk.CompressedOffset;
            var chunkHeader = new FCompressedChunkHeader();
            chunkHeader.Deserialize(inputBinaryReader);

            var sumUncompressedSize = 0;
            var blocks = new List<FCompressedChunkBlock>();
            var blockCount = chunkHeader.BlockCount;


            while (sumUncompressedSize < chunkHeader.Summary.UncompressedSize)
            {
                var block = new FCompressedChunkBlock();
                block.Deserialize(inputBinaryReader);
                blocks.Add(block);
                sumUncompressedSize += block.UncompressedSize;
            }

            foreach (var block in blocks)
            {
                var compressedData = inputBinaryReader.ReadBytes(block.CompressedSize);
                using var zlibStream = new ZLibStream(new MemoryStream(compressedData), CompressionMode.Decompress);
                zlibStream.CopyTo(uncompressOutputStream);
            }

            Debug.Assert(uncompressOutputStream.Position == uncompressProgress + chunk.UncompressedSize);
            uncompressProgress += chunk.UncompressedSize;
        }

        DeserializationState |= DeserializationState.Inflated;

        Debug.Assert(uncompressOutputStream.Position == uncompressOutputStream.Length);
        outputStream.Position = firstUncompressedOffset;
        outputStream.Write(uncompressedDataBuffer);
    }

    private void ProcessDecryptedData(Stream outputStream, BinaryReader inputBinaryReader, IDecrypterProvider decrypterProvider)
    {
        byte[] decryptedData;
        try
        {
            decryptedData = DecryptData(inputBinaryReader, decrypterProvider);
        }
        catch (InvalidDataException)
        {
            return;
        }

        var decryptedDataReader = new BinaryReader(new MemoryStream(decryptedData));

        decryptedDataReader.BaseStream.Position = FileSummary.CompressedChunkInfoOffset;
        FileSummary.CompressedChunks.Deserialize(decryptedDataReader);
        // The depends table is always empty. So The depends table marks the start of where the uncompressed data should go.
        Debug.Assert(FileSummary.CompressedChunks.First().UncompressedOffset == FileSummary.DependsOffset);
        outputStream.Write(decryptedData);
        DeserializationState |= DeserializationState.Decrypted;
    }

    private void ProcessFileSummary(Stream outputStream, BinaryReader inputBinaryReader)
    {
        FileSummary.Deserialize(inputBinaryReader);
        inputBinaryReader.BaseStream.Position = 0;
        var fileSummaryBytes = inputBinaryReader.ReadBytes(FileSummary.NameOffset);
        outputStream.Write(fileSummaryBytes);

        DeserializationState |= DeserializationState.Header;
        if (!FileSummary.CompressionFlags.HasFlag(ECompressionFlags.COMPRESS_ZLIB))
        {
            throw new InvalidDataException("Package compression type is unsupported ");
        }
    }


    private byte[] DecryptData(BinaryReader reader, IDecrypterProvider decrypterProvider)
    {
        reader.BaseStream.Seek(FileSummary.NameOffset, SeekOrigin.Begin);
        var encryptedData = reader.ReadBytes(EncryptedSize);
        if (encryptedData.Length != EncryptedSize)
        {
            throw new InvalidDataException("Failed to read the encrypted data from the stream");
        }

        var validDecryptor = decrypterProvider.DecryptionKeys.Select(decrypterProvider.GetCryptoTransform)
            .FirstOrDefault(x => VerifyDecryptor(x, encryptedData));
        if (validDecryptor == null)
        {
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
        var blockOffset = FileSummary.CompressedChunkInfoOffset % 16;
        var blockStart = FileSummary.CompressedChunkInfoOffset - blockOffset;
        var chunkInfoBytes = new byte[32];

        decryptor.TransformBlock(encryptedData, blockStart, 32, chunkInfoBytes, 0);

        var binaryReader = new BinaryReader(new MemoryStream(new Span<byte>(chunkInfoBytes)[blockOffset..].ToArray()));
        var chunkInfoLength = binaryReader.ReadInt32();
        var uncompressedOffsetFirstChunk = binaryReader.ReadInt32();
        return chunkInfoLength >= 1 && uncompressedOffsetFirstChunk == FileSummary.DependsOffset;
    }
}