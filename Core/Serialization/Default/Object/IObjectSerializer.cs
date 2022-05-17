namespace Core.Serialization.Default.Object;

public interface IObjectSerializer<T>
{
    void DeserializeObject(T obj, Stream objectStream);
    void SerializeObject(T obj, Stream objectStream);
}