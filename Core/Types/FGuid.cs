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

    /// <summary>
    ///     Reads the 16 bytes of a GUID from the stream as four Uint32 values.
    /// </summary>
    /// <param name="reader"></param>
    public void Deserialize(Stream reader)
    {
        A = reader.ReadUInt32();
        B = reader.ReadUInt32();
        C = reader.ReadUInt32();
        D = reader.ReadUInt32();
    }

    /// <summary>
    ///     Writes the  16 bytes of the GUID to the stream as four Uint32 values
    /// </summary>
    /// <param name="writer"></param>
    public void Serialize(Stream writer)
    {
        writer.WriteUInt32(A);
        writer.WriteUInt32(B);
        writer.WriteUInt32(C);
        writer.WriteUInt32(D);
    }
}