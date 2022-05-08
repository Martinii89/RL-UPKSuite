namespace Core.Types.PackageTables;

/// <summary>
///     A Thumbnail table contains metadata about all the allocated textures in a package. This will not be present in a
///     cooked package
/// </summary>
public class ThumbnailTable
{
    /// <summary>
    ///     The list of thumbnail metadatas
    /// </summary>
    public List<ThumbnailTableItem> Thumbnails { get; } = new();
}

/// <summary>
///     Metadata about a allocated texture in a package
/// </summary>
public class ThumbnailTableItem
{
    /// <summary>
    ///     Construct the metadata fields
    /// </summary>
    /// <param name="name"></param>
    /// <param name="group"></param>
    /// <param name="dataOffset"></param>
    public ThumbnailTableItem(string name, string group, int dataOffset)
    {
        Name = name;
        Group = group;
        DataOffset = dataOffset;
    }

    /// <summary>
    ///     The name of the texture
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     The Group of the texture.
    /// </summary>
    public string Group { get; }

    /// <summary>
    ///     The location of the texture data
    /// </summary>
    public int DataOffset { get; }

    /// <summary>
    ///     The serial data of a thumbnail. Currently always null
    /// </summary>
    public ThumbnailData? ThumbnailData { get; set; }
}

/// <summary>
///     The  data of a thumbnail
/// </summary>
public class ThumbnailData
{
    /// <summary>
    ///     Construts a thumbnail from deserialized data
    /// </summary>
    /// <param name="sizeX"></param>
    /// <param name="sizeY"></param>
    /// <param name="data"></param>
    public ThumbnailData(int sizeX, int sizeY, byte[] data)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        Data = data;
        DataSize = data?.Length ?? 0;
    }

    /// <summary>
    ///     The width of the image
    /// </summary>
    public int SizeX { get; }

    /// <summary>
    ///     The height of the image
    /// </summary>
    public int SizeY { get; }

    /// <summary>
    ///     The total size of the image data
    /// </summary>
    public int DataSize { get; }

    /// <summary>
    ///     The image data
    /// </summary>
    public byte[] Data { get; }
}