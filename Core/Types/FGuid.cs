namespace Core.Types;

/// <summary>
///     Unreal specific GUID struct.
/// </summary>
public class FGuid
{
    /// <summary>
    ///     First GUID part
    /// </summary>
    public uint A { get; internal set; }

    /// <summary>
    ///     Second GUID part
    /// </summary>
    public uint B { get; internal set; }

    /// <summary>
    ///     Third GUID part
    /// </summary>
    public uint C { get; internal set; }

    /// <summary>
    ///     Fourth GUID part
    /// </summary>
    public uint D { get; internal set; }
}