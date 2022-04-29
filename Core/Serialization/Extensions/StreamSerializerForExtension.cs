namespace Core.Serialization;

/// <summary>
///     Extensions for using the serializers to read and write arrays
/// </summary>
public static class StreamSerializerForExtension
{
    /// <summary>
    ///     Decides how the array will be serialized
    /// </summary>
    public enum ArraySizeSerialization
    {
        /// <summary>
        ///     Writes the amount of elements to the stream right before the elements
        /// </summary>
        PrependSize,

        /// <summary>
        ///     The amount of elements will not be written to the stream.
        /// </summary>
        NoSize
    }

    /// <summary>
    ///     Read a array of values from the stream using the typed serializer. If the size is given that will be used. If not
    ///     the size will be read from the stream.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="serializer"></param>
    /// <param name="size">The number of elements to read. if null the value will be read from the stream </param>
    /// <returns></returns>
    public static T[] ReadTArray<T>(this IStreamSerializerFor<T> serializer, Stream stream, int? size = null)
    {
        var result = new T[size ?? stream.ReadInt32()];

        for (var i = 0; i < result.Length; i++)
        {
            result[i] = serializer.Deserialize(stream);
        }

        return result;
    }


    /// <summary>
    ///     Writes a array of values to the stream using a given typed serializer. The output can be prepended with the amount
    ///     of elements, most arrays
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="serializer"></param>
    /// <param name="values"></param>
    /// <param name="sizeOtion"></param>
    public static void WriteTArray<T>(this IStreamSerializerFor<T> serializer, Stream stream, T[] values,
        ArraySizeSerialization sizeOtion = ArraySizeSerialization.PrependSize)
    {
        if (sizeOtion == ArraySizeSerialization.PrependSize)
        {
            stream.WriteInt32(values.Length);
        }

        foreach (var value in values)
        {
            serializer.Serialize(stream, value);
        }
    }
}