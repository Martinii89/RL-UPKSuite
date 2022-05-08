namespace Core.Types;

/// <summary>
///     Wrapper for strings stored in unreal packages.
/// </summary>
public class FString
{
    internal string InnerString { get; set; } = string.Empty;


    /// <summary>
    ///     Returns the deserialized string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return InnerString;
    }
}