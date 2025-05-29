namespace RlUpk.Core.Serialization.Abstraction;

/// <summary>
///     A IObjectSerializerFactory can return a ObjectSerializer for a given type
/// </summary>
public interface IObjectSerializerFactory
{
    /// <summary>
    ///     Returns a serializer for the given type or null if no serializer is found
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IObjectSerializer? GetSerializer(Type type);
}