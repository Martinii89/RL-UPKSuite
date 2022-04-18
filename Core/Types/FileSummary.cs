using Core.Compression;
using Core.Types.FileSummeryInner;

namespace Core.Types;

public class FileSummary
{
    private const uint PACKAGE_FILE_TAG = 0x9E2A83C1;

    public uint Tag { get; private set; }
    public ushort FileVersion { get; private set; }
    public ushort LicenseeVersion { get; private set; }
    public int TotalHeaderSize { get; private set; }
    public FString FolderName { get; private set; } = new();
    public uint PackageFlags { get; private set; }
    public int NameCount { get; private set; }
    public int NameOffset { get; private set; }
    public int ExportCount { get; private set; }
    public int ExportOffset { get; private set; }
    public int ImportCount { get; private set; }
    public int ImportOffset { get; private set; }
    public int DependsOffset { get; private set; }
    public int ImportExportGuidsOffset { get; private set; }
    public int ImportGuidsCount { get; private set; }
    public int ExportGuidsCount { get; private set; }
    public int ThumbnailTableOffset { get; private set; }
    public FGuid Guid { get; private set; } = new();
    public TArray<FGenerationInfo> Generations { get; private set; } = new();
    public uint EngineVersion { get; private set; }
    public uint CookerVersion { get; private set; }
    public ECompressionFlags CompressionFlags { get; private set; }
    internal TArray<FCompressedChunkInfo> CompressedChunks { get; private set; } = new();

    // ReSharper disable once NotAccessedField.Local
    private int _unknown5; // Probably a hash
    public TArray<FString> AdditionalPackagesToCook { get; private set; } = new();
    public TArray<FTextureType> TextureAllocations { get; private set; } = new();

    // Number of bytes of (pos % 0xFF) at the end of the decrypted data, I don't know why it's needed
    internal int GarbageSize { get; private set; }

    // Offset to TArray<FCompressedChunkInfo> in decrypted data
    internal int CompressedChunkInfoOffset { get; private set; }

    // Size of the last AES block in the encrypted data
    internal int LastBlockSize { get; private set; }

    public void Deserialize(BinaryReader Reader)
    {
        Tag = Reader.ReadUInt32();
        if (Tag != PACKAGE_FILE_TAG)
        {
            throw new Exception("Not a valid Unreal Engine package.");
        }

        FileVersion = Reader.ReadUInt16();
        LicenseeVersion = Reader.ReadUInt16();

        TotalHeaderSize = Reader.ReadInt32();
        FolderName.Deserialize(Reader);
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