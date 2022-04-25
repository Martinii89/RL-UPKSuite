namespace Core.Serialization;

public interface ISerializerCollection
{
    IStreamSerializerFor<T>? GetSerializerFor<T>();
}