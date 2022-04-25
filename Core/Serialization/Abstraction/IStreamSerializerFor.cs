namespace Core.Serialization;

public interface IStreamSerializerFor<T>
{
    T Deserialize(Stream stream);
    void Serialize(Stream stream, ref T value);
}