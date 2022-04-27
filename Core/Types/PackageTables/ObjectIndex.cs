namespace Core.Types.PackageTables;

/// <summary>
///     References either a import or export object.
///     Values larger than zero are export objects. The real index in the export table will be  (index - 1 )
///     Values less than zero are import objects. The real index in the import table will be ( - index - 1 )
///     Zero is a null reference
/// </summary>
public class ObjectIndex
{
    /// <summary>
    ///     Constructs a null reference
    /// </summary>
    public ObjectIndex()
    {
    }

    /// <summary>
    ///     Constructs a reference with a given index
    /// </summary>
    /// <param name="index"></param>
    public ObjectIndex(int index)
    {
        Index = index;
    }

    /// <summary>
    ///     The reference index. Larger than zero is a export. Less than zero is a import
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    ///     Reads a int32 value from the stream and assigns this as the Index
    /// </summary>
    /// <param name="stream"></param>
    public void Deserialize(Stream stream)
    {
        Index = stream.ReadInt32();
    }

    /// <summary>
    ///     Writes the index to the stream as a in32 value
    /// </summary>
    /// <param name="stream"></param>
    public void Serialize(Stream stream)
    {
        stream.WriteInt32(Index);
    }
}