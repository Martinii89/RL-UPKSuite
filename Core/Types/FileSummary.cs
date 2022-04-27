using Core.Compression;
using Core.Types.FileSummeryInner;

namespace Core.Types;

/// <summary>
///     A FileSummary represents the first part of a unreal package.
///     It contains all the required data to process the serial data of the package
/// </summary>
public class FileSummary
{
    /// <summary>
    ///     Magic constant all compatible packages start with
    /// </summary>
    public const uint PackageFileTag = 0x9E2A83C1;

    // ReSharper disable once NotAccessedField.Local
#pragma warning disable IDE0052 // Remove unread private members
    private int _unknown5; // Probably a hash
#pragma warning restore IDE0052 // Remove unread private members

    /// <summary>
    ///     The Magic package header tag. Should always be equal to <see cref="PackageFileTag" />
    /// </summary>
    public uint Tag { get; private set; }

    /// <summary>
    ///     The FileVersion of this package
    /// </summary>
    public ushort FileVersion { get; private set; }

    /// <summary>
    ///     The licensee version used to save this package. Can be used to identify the origin of this package
    /// </summary>
    public ushort LicenseeVersion { get; private set; }

    /// <summary>
    ///     The total header size is the sum of the FileSummary the encrypted part and the garbage padding
    /// </summary>
    public int TotalHeaderSize { get; private set; }

    /// <summary>
    ///     Folder name. Unused for cooked packages and always "None"
    /// </summary>
    public string FolderName { get; private set; } = string.Empty;

    /// <summary>
    ///     BitFlag defining some properties for this package. No clue what they actually mean in psyonix cooked packages.
    /// </summary>
    public uint PackageFlags { get; private set; }

    /// <summary>
    ///     The number of names in the names table
    /// </summary>
    public int NameCount { get; private set; }

    /// <summary>
    ///     The offset where the name table starts in the file
    /// </summary>
    public int NameOffset { get; private set; }

    /// <summary>
    ///     The number of exported objects from this package
    /// </summary>
    public int ExportCount { get; private set; }

    /// <summary>
    ///     The offset where the export table starts in the file
    /// </summary>
    public int ExportOffset { get; private set; }

    /// <summary>
    ///     The number of imported objects in this package
    /// </summary>
    public int ImportCount { get; private set; }

    /// <summary>
    ///     The offset where the import table in the file
    /// </summary>
    public int ImportOffset { get; private set; }

    /// <summary>
    ///     The offset to the depends array in the file. Unused for cooked (Psyonix?) packages
    /// </summary>
    public int DependsOffset { get; private set; }

    /// <summary>
    ///     The offset to the import/export guids data
    /// </summary>
    public int ImportExportGuidsOffset { get; private set; }

    /// <summary>
    ///     The number of guid imports
    /// </summary>
    public int ImportGuidsCount { get; private set; }

    /// <summary>
    ///     The number of guid exports
    /// </summary>
    public int ExportGuidsCount { get; private set; }

    /// <summary>
    ///     The offset to the Thumbnails table in the file
    /// </summary>
    public int ThumbnailTableOffset { get; private set; }

    /// <summary>
    ///     The package GUID
    /// </summary>
    public FGuid Guid { get; } = new();

    /// <summary>
    ///     Data about previous versions of this package
    /// </summary>
    public TArray<FGenerationInfo> Generations { get; } = new();

    /// <summary>
    ///     The version of the engine that serialized this package
    /// </summary>
    public uint EngineVersion { get; private set; }

    /// <summary>
    ///     The version of the cooker that cooked this package
    /// </summary>
    public uint CookerVersion { get; private set; }

    /// <summary>
    ///     Flag denoting if this package is compressed and what kind of compression is used
    /// </summary>
    public ECompressionFlags CompressionFlags { get; private set; }

    /// <summary>
    ///     Offset into the file to where the compression flag is set
    /// </summary>
    public int CompressionFlagsOffset { get; private set; }

    internal TArray<FCompressedChunkInfo> CompressedChunks { get; private set; } = new();

    /// <summary>
    ///     List of other packages required by this package
    /// </summary>
    public TArray<FString> AdditionalPackagesToCook { get; } = new();

    /// <summary>
    ///     Textures stored in this package
    /// </summary>
    public TArray<FTextureType> TextureAllocations { get; } = new();

    // Number of bytes of (pos % 0xFF) at the end of the decrypted data, I don't know why it's needed
    internal int GarbageSize { get; private set; }

    // Offset to TArray<FCompressedChunkInfo> in decrypted data
    internal int CompressedChunkInfoOffset { get; private set; }

    // Size of the last AES block in the encrypted data
    internal int LastBlockSize { get; private set; }

    /// <summary>
    ///     Deserialize the summary. Can throw if file tag is wrong.
    /// </summary>
    /// <param name="reader"></param>
    /// <exception cref="Exception">Thrown when the tag doesn't match</exception>
    public void Deserialize(BinaryReader reader)
    {
        Tag = reader.ReadUInt32();
        if (Tag != PackageFileTag) throw new Exception("Not a valid Unreal Engine package");

        FileVersion = reader.ReadUInt16();
        LicenseeVersion = reader.ReadUInt16();

        TotalHeaderSize = reader.ReadInt32();
        var folderName = new FString();
        folderName.Deserialize(reader);
        FolderName = folderName.ToString();
        PackageFlags = reader.ReadUInt32();

        NameCount = reader.ReadInt32();
        NameOffset = reader.ReadInt32();

        ExportCount = reader.ReadInt32();
        ExportOffset = reader.ReadInt32();

        ImportCount = reader.ReadInt32();
        ImportOffset = reader.ReadInt32();

        DependsOffset = reader.ReadInt32();

        ImportExportGuidsOffset = reader.ReadInt32();
        ImportGuidsCount = reader.ReadInt32();
        ExportGuidsCount = reader.ReadInt32();
        ThumbnailTableOffset = reader.ReadInt32();

        Guid.Deserialize(reader);

        Generations.Deserialize(reader);

        EngineVersion = reader.ReadUInt32();
        CookerVersion = reader.ReadUInt32();

        CompressionFlagsOffset = (int)reader.BaseStream.Position;
        CompressionFlags = (ECompressionFlags)reader.ReadUInt32();

        CompressedChunks = new TArray<FCompressedChunkInfo>(() => new FCompressedChunkInfo(this));
        CompressedChunks.Deserialize(reader);

        _unknown5 = reader.ReadInt32();

        AdditionalPackagesToCook.Deserialize(reader);
        TextureAllocations.Deserialize(reader);

        GarbageSize = reader.ReadInt32();
        CompressedChunkInfoOffset = reader.ReadInt32();
        LastBlockSize = reader.ReadInt32();
    }
}