using Core.Serialization;

namespace Core.Types.FileSummeryInner;

/// <summary>
///     A FTextureType contains metadata about a texture stored in a package.
/// </summary>
public class FTextureType : IBinaryDeserializableClass
{
    /// <summary>
    ///     The width of the image in pixels
    /// </summary>
    public int SizeX { get; private set; }

    /// <summary>
    ///     The height of the image in pixels
    /// </summary>
    public int SizeY { get; private set; }

    /// <summary>
    ///     The number of mipmaps in the texture
    /// </summary>
    public int NumMips { get; private set; }

    /// <summary>
    ///     A int probably representing a enum for different image formats
    /// </summary>
    public int Format { get; private set; }

    /// <summary>
    ///     Another flag of some kind
    /// </summary>
    public int TexCreateFlags { get; private set; }

    /// <summary>
    ///     Indexes into the export table. Probably the objects that reference this texture?
    /// </summary>
    public TArray<int> ExportIndices { get; } = new();

    /// <summary>
    ///     Deserialize the members from the stream
    /// </summary>
    /// <param name="reader"></param>
    public void Deserialize(BinaryReader reader)
    {
        SizeX = reader.ReadInt32();
        SizeY = reader.ReadInt32();
        ;
        NumMips = reader.ReadInt32();
        ;
        Format = reader.ReadInt32();
        ;
        TexCreateFlags = reader.ReadInt32();
        ;
        ExportIndices.Deserialize(reader);
    }
}