﻿namespace Core.Serialization;

/// <summary>
///     A IStreamSerializer implements serialization and deserialization from a <see cref="Stream" />
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IStreamSerializer<T>
{
    /// <summary>
    ///     Constructs a T from the stream
    /// </summary>
    /// <param name="stream">The stream to read from</param>
    /// <returns>The constructed object</returns>
    public T Deserialize(Stream stream);

    /// <summary>
    ///     Write the value to the stream
    /// </summary>
    /// <param name="stream">The stream to write to </param>
    /// <param name="value">The value to write</param>
    public void Serialize(Stream stream, T value);
}