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
    internal int Unknown5 { get; set; } // Probably a hash
#pragma warning restore IDE0052 // Remove unread private members

    /// <summary>
    ///     The Magic package header tag. Should always be equal to <see cref="PackageFileTag" />
    /// </summary>
    public uint Tag { get; internal set; }

    /// <summary>
    ///     The FileVersion of this package
    /// </summary>
    public ushort FileVersion { get; internal set; }

    /// <summary>
    ///     The licensee version used to save this package. Can be used to identify the origin of this package
    /// </summary>
    public ushort LicenseeVersion { get; internal set; }

    /// <summary>
    ///     The total header size is the sum of the FileSummary the encrypted part and the garbage padding
    /// </summary>
    public int TotalHeaderSize { get; internal set; }

    /// <summary>
    ///     Folder name. Unused for cooked packages and always "None"
    /// </summary>
    public string FolderName { get; internal set; } = string.Empty;

    /// <summary>
    ///     BitFlag defining some properties for this package. No clue what they actually mean in psyonix cooked packages.
    /// </summary>
    public uint PackageFlags { get; internal set; }
    
    public int PackageFlagsFlagsOffset { get; set; }

    /// <summary>
    ///     The number of names in the names table
    /// </summary>
    public int NameCount { get; internal set; }

    /// <summary>
    ///     The offset where the name table starts in the file
    /// </summary>
    public int NameOffset { get; internal set; }

    /// <summary>
    ///     The number of exported objects from this package
    /// </summary>
    public int ExportCount { get; internal set; }

    /// <summary>
    ///     The offset where the export table starts in the file
    /// </summary>
    public int ExportOffset { get; internal set; }

    /// <summary>
    ///     The number of imported objects in this package
    /// </summary>
    public int ImportCount { get; internal set; }

    /// <summary>
    ///     The offset where the import table in the file
    /// </summary>
    public int ImportOffset { get; internal set; }

    /// <summary>
    ///     The offset to the depends array in the file. Unused for cooked (Psyonix?) packages
    /// </summary>
    public int DependsOffset { get; internal set; }

    /// <summary>
    ///     The offset to the import/export guids data
    /// </summary>
    public int ImportExportGuidsOffset { get; internal set; }

    /// <summary>
    ///     The number of guid imports
    /// </summary>
    public int ImportGuidsCount { get; internal set; }

    /// <summary>
    ///     The number of guid exports
    /// </summary>
    public int ExportGuidsCount { get; internal set; }

    /// <summary>
    ///     The offset to the Thumbnails table in the file
    /// </summary>
    public int ThumbnailTableOffset { get; internal set; }

    /// <summary>
    ///     The package GUID
    /// </summary>
    public FGuid Guid { get; set; } = new();

    /// <summary>
    ///     Data about previous versions of this package
    /// </summary>
    public List<FGenerationInfo> Generations { get; set; } = new();

    /// <summary>
    ///     The version of the engine that serialized this package
    /// </summary>
    public uint EngineVersion { get; internal set; }

    /// <summary>
    ///     The version of the cooker that cooked this package
    /// </summary>
    public uint CookerVersion { get; internal set; }

    /// <summary>
    ///     Flag denoting if this package is compressed and what kind of compression is used
    /// </summary>
    public ECompressionFlags CompressionFlags { get; internal set; }

    /// <summary>
    ///     Offset into the file to where the compression flag is set
    /// </summary>
    public int CompressionFlagsOffset { get; internal set; }

    internal List<FCompressedChunk> CompressedChunks { get; set; } = new();

    /// <summary>
    ///     List of other packages required by this package
    /// </summary>
    public List<FString> AdditionalPackagesToCook { get; set; } = new();

    /// <summary>
    ///     Textures stored in this package
    /// </summary>
    public List<FTextureType> TextureAllocations { get; set; } = new();

    public static FileSummary CreateWithDefaults()
    {
        return new FileSummary()
        {
            Tag = FileSummary.PackageFileTag,
            FileVersion = 868,
            FolderName = "None",
            Generations = [new FGenerationInfo()
            {
                ExportCount = 1,
                NameCount = 7,
                NetObjectCount = 1
            }]
        };
    }
}