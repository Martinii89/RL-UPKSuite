namespace Core.Serialization;

/// <summary>
///     This class can be deserialized from a specified stream.
/// </summary>
public interface IBinaryDeserializableClass
{
    /// <summary>
    ///     Reads values from the stream and sets serialized properties of the implementing class
    /// </summary>
    /// <param name="reader"></param>
    void Deserialize(Stream reader);
}

/// <summary>
///     This class can be serialized to a stream
/// </summary>
public interface IBinarySerializableClass
{
    /// <summary>
    ///     Write the serial data to the stream
    /// </summary>
    /// <param name="writer"></param>
    void Serialize(Stream writer);
}