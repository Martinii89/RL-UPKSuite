using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using Core.RocketLeague.Decryption;
using Core.Types;
using Core.Types.FileSummeryInner;

namespace Core.RocketLeague;

/// <summary>
///     Indicates the result of a <see cref="PackageUnpacker" /> unpacking.
/// </summary>
[Flags]
public enum DeserializationState
{
    /// <summary>
    ///     No unpacking has been done
    /// </summary>
    None = 0,

    /// <summary>
    ///     Header has been deserialized
    /// </summary>
    Header = 1,

    /// <summary>
    ///     Encrypted data has been decrypted
    /// </summary>
    Decrypted = 2,

    /// <summary>
    ///     Compressed data has been uncompressed
    /// </summary>
    Inflated = 4,

    /// <summary>
    ///     Unpacking was successful
    /// </summary>
    Success = Header | Decrypted | Inflated
}

/// <summary>
///     Unpacks a Rocket League encrypted and compressed UnrealPackage.
/// </summary>
public class PackageUnpacker
{
    /// <summary>
    ///     Starts unpacking the data from the input stream. It does little to no checks to verify it is a RocketLeague
    ///     package. Check the Deserialization state or the Valid property to know if unpacking was successful
    /// </summary>
    /// <param name="inputStream">The package content stream</param>
    /// <param name="outputStream">Unpacked package will be written to this</param>
    /// <param name="decrypterProvider">Required to unpack the decrypted data</param>
    public PackageUnpacker(Stream inputStream, Stream outputStream, IDecrypterProvider decrypterProvider)
    {
        var inputBinaryReader = new BinaryReader(inputStream);
        var outputBinaryWriter = new BinaryWriter(outputStream);

        ProcessFileSummary(outputBinaryWriter, inputStream);

        ProcessDecryptedData(outputBinaryWriter, inputBinaryReader, decrypterProvider);
        if (!DeserializationState.HasFlag(DeserializationState.Decrypted))
        {
            return;
        }

        ProcessCompressedData(outputBinaryWriter, inputBinaryReader);
    }

    /// <summary>
    ///     The Parsed FileSummary of this package
    /// </summary>
    public FileSummary FileSummary { get; } = new();

    private FileCompressionMetaData FileCompressionMetaData { get; } = new();

    /// <summary>
    ///     The state of unpacking. Anything but DeserializationState.Inflated would indicate some kind of error.
    /// </summary>
    public DeserializationState DeserializationState { get; private set; } = DeserializationState.None;

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
    public bool Valid => DeserializationState.HasFlag(DeserializationState.Inflated);

    private void ProcessCompressedData(BinaryWriter outputStream, BinaryReader inputBinaryReader)
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
            chunkHeader.Deserialize(inputBinaryReader.BaseStream);

            var sumUncompressedSize = 0;
            var blocks = new List<FCompressedChunkBlock>();
            var blockCount = chunkHeader.BlockCount;


            while (sumUncompressedSize < chunkHeader.Summary.UncompressedSize)
            {
                var block = new FCompressedChunkBlock();
                block.Deserialize(inputBinaryReader.BaseStream);
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
        outputStream.BaseStream.Position = firstUncompressedOffset;
        outputStream.Write(uncompressedDataBuffer);

        // Reset the compression flag to indicate this package is no longer compressed.
        outputStream.BaseStream.Position = FileSummary.CompressionFlagsOffset;
        outputStream.Write((int) ECompressionFlags.CompressNone);
    }

    private void ProcessDecryptedData(BinaryWriter outputStream, BinaryReader inputBinaryReader, IDecrypterProvider decrypterProvider)
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

        decryptedDataReader.BaseStream.Position = FileCompressionMetaData.CompressedChunkInfoOffset;
        FileSummary.CompressedChunks.Deserialize(decryptedDataReader.BaseStream);
        // The depends table is always empty. So The depends table marks the start of where the uncompressed data should go.
        Debug.Assert(FileSummary.CompressedChunks.First().UncompressedOffset == FileSummary.DependsOffset);
        outputStream.Write(decryptedData);
        DeserializationState |= DeserializationState.Decrypted;
    }

    private void ProcessFileSummary(BinaryWriter outputStream, Stream inpuutStream)
    {
        FileSummary.Deserialize(inpuutStream);
        FileCompressionMetaData.Deserialize(inpuutStream);
        inpuutStream.Position = 0;
        var fileSummaryBytes = inpuutStream.ReadBytes(FileSummary.NameOffset);
        outputStream.Write(fileSummaryBytes);

        DeserializationState |= DeserializationState.Header;
        if (!FileSummary.CompressionFlags.HasFlag(ECompressionFlags.CompressZlib))
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
        var blockOffset = FileCompressionMetaData.CompressedChunkInfoOffset % 16;
        var blockStart = FileCompressionMetaData.CompressedChunkInfoOffset - blockOffset;
        var chunkInfoBytes = new byte[32];

        decryptor.TransformBlock(encryptedData, blockStart, 32, chunkInfoBytes, 0);

        var binaryReader = new BinaryReader(new MemoryStream(new Span<byte>(chunkInfoBytes)[blockOffset..].ToArray()));
        var chunkInfoLength = binaryReader.ReadInt32();
        var uncompressedOffsetFirstChunk = binaryReader.ReadInt32();
        return chunkInfoLength >= 1 && uncompressedOffsetFirstChunk == FileSummary.DependsOffset;
    }
}