namespace Core.Serialization.Default;

/// <inheritdoc />
public class Int32Serializer : IStreamSerializer<int>
{
    /// <inheritdoc />
    public int Deserialize(Stream stream)
    {
        return stream.ReadInt32();
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, int value)
    {
        stream.WriteInt32(value);
    }
}