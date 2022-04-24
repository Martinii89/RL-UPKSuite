namespace Core.Serialization;

/// <summary>
/// This class can be deserialized from a specified stream.
/// </summary>
public interface IBinaryDeserializableClass
{
    void Deserialize(BinaryReader reader);
}