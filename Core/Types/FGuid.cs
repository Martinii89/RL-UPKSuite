namespace RlUpk.Core.Types;

/// <summary>
///     Unreal specific GUID struct.
/// </summary>
public readonly struct FGuid
{
    /// <summary>
    ///     First GUID part
    /// </summary>
    public uint A { get; internal init; }

    /// <summary>
    ///     Second GUID part
    /// </summary>
    public uint B { get; internal init; }

    /// <summary>
    ///     Third GUID part
    /// </summary>
    public uint C { get; internal init; }

    /// <summary>
    ///     Fourth GUID part
    /// </summary>
    public uint D { get; internal init; }
}