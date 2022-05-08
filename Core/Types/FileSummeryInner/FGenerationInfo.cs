namespace Core.Types.FileSummeryInner;

/// <summary>
///     A FGenerationInfo is a member of a <see cref="FileSummary" /> If present it will give information about previous
///     versions of the package
/// </summary>
public class FGenerationInfo
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
}