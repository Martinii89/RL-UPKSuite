namespace Core.Types.FileSummeryInner;

/// <summary>
///     A FTextureType contains metadata about a texture stored in a package.
/// </summary>
public class FTextureType
{
    /// <summary>
    ///     The width of the image in pixels
    /// </summary>
    public int SizeX { get; internal set; }

    /// <summary>
    ///     The height of the image in pixels
    /// </summary>
    public int SizeY { get; internal set; }

    /// <summary>
    ///     The number of mipmaps in the texture
    /// </summary>
    public int NumMips { get; internal set; }

    /// <summary>
    ///     A int probably representing a enum for different image formats
    /// </summary>
    public int Format { get; internal set; }

    /// <summary>
    ///     Another flag of some kind
    /// </summary>
    public int TexCreateFlags { get; internal set; }

    /// <summary>
    ///     Indexes into the export table. Probably the objects that reference this texture?
    /// </summary>
    public List<int> ExportIndices { get; internal set; } = new();
}