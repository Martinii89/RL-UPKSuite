using Core.Compression;
using Core.Types.FileSummeryInner;

namespace Core.Types;

public class FileSummary
{
    public const uint PackageFileTag = 0x9E2A83C1;

    /// <summary>
    /// The Magic package header tag. Should always be equal to <see cref="PackageFileTag"/>
    /// </summary>
    public uint Tag { get; private set; }

    /// <summary>
    /// The FileVersion of this package
    /// </summary>
    public ushort FileVersion { get; private set; }

    /// <summary>
    /// The licensee version used to save this package. Can be used to identify the origin of this package
    /// </summary>
    public ushort LicenseeVersion { get; private set; }

    /// <summary>
    /// The total header size is the sum of the FileSummary the encrypted part and the garbage padding
    /// </summary>
    public int TotalHeaderSize { get; private set; }

    /// <summary>
    /// Folder name. Unused for cooked packages and always "None"
    /// </summary>
    public string FolderName { get; private set; } = string.Empty;

    /// <summary>
    /// BitFlag defining some properties for this package. No clue what they actually mean in psyonix cooked packages.
    /// </summary>
    public uint PackageFlags { get; private set; }

    /// <summary>
    /// The number of names in the names table
    /// </summary>
    public int NameCount { get; private set; }

    /// <summary>
    /// The offset where the name table starts in the file
    /// </summary>
    public int NameOffset { get; private set; }

    /// <summary>
    /// The number of exported objects from this package
    /// </summary>
    public int ExportCount { get; private set; }

    /// <summary>
    /// The offset where the export table starts in the file
    /// </summary>
    public int ExportOffset { get; private set; }

    /// <summary>
    /// The number of imported objects in this package
    /// </summary>
    public int ImportCount { get; private set; }

    /// <summary>
    /// The offset where the import table in the file
    /// </summary>
    public int ImportOffset { get; private set; }

    /// <summary>
    /// The offset to the depends array in the file. Unused for cooked (Psyonix?) packages
    /// </summary>
    public int DependsOffset { get; private set; }

    /// <summary>
    /// The offset to the import/export guids data
    /// </summary>
    public int ImportExportGuidsOffset { get; private set; }

    /// <summary>
    /// The number of guid imports
    /// </summary>
    public int ImportGuidsCount { get; private set; }

    /// <summary>
    /// The number of guid exports
    /// </summary>
    public int ExportGuidsCount { get; private set; }

    /// <summary>
    /// The offset to the Thumbnails table in the file
    /// </summary>
    public int ThumbnailTableOffset { get; private set; }

    /// <summary>
    /// The package GUID
    /// </summary>
    public FGuid Guid { get; private set; } = new();

    /// <summary>
    /// Data about previous versions of this package
    /// </summary>
    public TArray<FGenerationInfo> Generations { get; private set; } = new();

    /// <summary>
    /// The version of the engine that serialized this package
    /// </summary>
    public uint EngineVersion { get; private set; }

    /// <summary>
    /// The version of the cooker that cooked this package
    /// </summary>
    public uint CookerVersion { get; private set; }

    /// <summary>
    /// Flag denoting if this package is compressed and what kind of compression is used
    /// </summary>
    public ECompressionFlags CompressionFlags { get; private set; }

    /// <summary>
    /// Offset into the file to where the compression flag is set
    /// </summary>
    public int CompressionFlagsOffset { get; private set; }

    internal TArray<FCompressedChunkInfo> CompressedChunks { get; private set; } = new();

    // ReSharper disable once NotAccessedField.Local
    private int _unknown5; // Probably a hash

    /// <summary>
    /// List of other packages required by this package
    /// </summary>
    public TArray<FString> AdditionalPackagesToCook { get; private set; } = new();

    /// <summary>
    /// Textures stored in this package
    /// </summary>
    public TArray<FTextureType> TextureAllocations { get; private set; } = new();

    // Number of bytes of (pos % 0xFF) at the end of the decrypted data, I don't know why it's needed
    internal int GarbageSize { get; private set; }

    // Offset to TArray<FCompressedChunkInfo> in decrypted data
    internal int CompressedChunkInfoOffset { get; private set; }

    // Size of the last AES block in the encrypted data
    internal int LastBlockSize { get; private set; }

    /// <summary>
    /// Deserialize the summary. Can throw if file tag is wrong.
    /// </summary>
    /// <param name="Reader"></param>
    /// <exception cref="Exception">Thrown when the tag doesn't match</exception>
    public void Deserialize(BinaryReader Reader)
    {
        Tag = Reader.ReadUInt32();
        if (Tag != PackageFileTag)
        {
            throw new Exception("Not a valid Unreal Engine package");
        }

        FileVersion = Reader.ReadUInt16();
        LicenseeVersion = Reader.ReadUInt16();

        TotalHeaderSize = Reader.ReadInt32();
        var folderName = new FString();
        folderName.Deserialize(Reader);
        FolderName = folderName.ToString();
        PackageFlags = Reader.ReadUInt32();

        NameCount = Reader.ReadInt32();
        NameOffset = Reader.ReadInt32();

        ExportCount = Reader.ReadInt32();
        ExportOffset = Reader.ReadInt32();

        ImportCount = Reader.ReadInt32();
        ImportOffset = Reader.ReadInt32();

        DependsOffset = Reader.ReadInt32();

        ImportExportGuidsOffset = Reader.ReadInt32();
        ImportGuidsCount = Reader.ReadInt32();
        ExportGuidsCount = Reader.ReadInt32();
        ThumbnailTableOffset = Reader.ReadInt32();

        Guid.Deserialize(Reader);

        Generations.Deserialize(Reader);

        EngineVersion = Reader.ReadUInt32();
        CookerVersion = Reader.ReadUInt32();

        CompressionFlagsOffset = (int) Reader.BaseStream.Position;
        CompressionFlags = (ECompressionFlags) (Reader.ReadUInt32());

        CompressedChunks = new TArray<FCompressedChunkInfo>(() => new FCompressedChunkInfo(this));
        CompressedChunks.Deserialize(Reader);

        _unknown5 = Reader.ReadInt32();

        AdditionalPackagesToCook.Deserialize(Reader);
        TextureAllocations.Deserialize(Reader);

        GarbageSize = Reader.ReadInt32();
        CompressedChunkInfoOffset = Reader.ReadInt32();
        LastBlockSize = Reader.ReadInt32();
    }
}