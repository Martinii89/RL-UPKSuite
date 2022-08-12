namespace Core.Serialization;

/// <summary>
///     A ISerializerCollection provides a type safe retrieval of <see cref="IStreamSerializer{T}" /> instances.
///     How these are retrieved is up to the implementation.
/// </summary>
public interface ISerializerCollection
{
    /// <summary>
    ///     Returns a <see cref="IStreamSerializer{T}" /> instance or null for a given type
    /// </summary>
    /// <typeparam name="T">The type to return a serializer for</typeparam>
    /// <returns></returns>
    IStreamSerializer<T>? GetSerializerFor<T>();
}