namespace Core.Serialization;

/// <summary>
/// This class can be deserialized from a specified stream.
/// </summary>
public interface IBinaryDeserializableClass
{
    void Deserialize(BinaryReader reader);
}

/// <summary>
/// This class can be serialized to a stream
/// </summary>
public interface IBinarySerializableClass
{
    void Serialize(Stream reader);
}