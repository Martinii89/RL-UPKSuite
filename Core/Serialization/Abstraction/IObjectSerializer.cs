namespace Core.Serialization.Abstraction;

/// <summary>
///     A IObjectSerializer reads and writes the object serial data
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IObjectSerializer<in T>
{
    /// <summary>
    ///     Read the object properties from the stream
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="objectStream"></param>
    void DeserializeObject(T obj, Stream objectStream);

    /// <summary>
    ///     Write the object properties to the stream
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="objectStream"></param>
    void SerializeObject(T obj, Stream objectStream);
}