namespace Core.Types;

/// <summary>
/// Unreal specific GUID struct.
/// </summary>
public class FGuid
{
    /// <summary>
    /// First GUID part
    /// </summary>
    public uint A { get; private set; }

    /// <summary>
    /// Second GUID part
    /// </summary>
    public uint B { get; private set; }

    /// <summary>
    /// Third GUID part
    /// </summary>
    public uint C { get; private set; }

    /// <summary>
    /// Fourth GUID part
    /// </summary>
    public uint D { get; private set; }

    public void Deserialize(BinaryReader reader)
    {
        A = reader.ReadUInt32();
        B = reader.ReadUInt32();
        C = reader.ReadUInt32();
        D = reader.ReadUInt32();
    }
}