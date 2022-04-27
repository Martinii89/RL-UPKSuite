using Core.Serialization;

namespace Core.Types.FileSummeryInner;

/// <summary>
///     A FGenerationInfo is a member of a <see cref="FileSummary" /> If present it will give information about previous
///     versions of the package
/// </summary>
public class FGenerationInfo : IBinaryDeserializableClass
{
    /// <summary>
    ///     How many Exports there was
    /// </summary>
    public int ExportCount { get; internal set; }

    /// <summary>
    ///     How many names there were
    /// </summary>
    public int NameCount { get; internal set; }

    /// <summary>
    ///     How many NetObjects there were
    /// </summary>
    public int NetObjectCount { get; internal set; }

    /// <summary>
    ///     Deserialize the members from the stream
    /// </summary>
    /// <param name="reader"></param>
    public void Deserialize(Stream reader)
    {
        ExportCount = reader.ReadInt32();
        NameCount = reader.ReadInt32();
        NetObjectCount = reader.ReadInt32();
    }
}