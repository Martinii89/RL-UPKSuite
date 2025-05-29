using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Extensions;

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
    ///     Reads a tarray from the stream and returns it as a List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializer"></param>
    /// <param name="stream"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static List<T> ReadTArrayToList<T>(this IStreamSerializer<T> serializer, Stream stream, int? size = null)
    {
        size ??= stream.ReadInt32();
        var result = new List<T>
        {
            Capacity = size.Value
        };

        for (var i = 0; i < size; i++)
        {
            result.Add(serializer.Deserialize(stream));
        }

        return result;
    }

    /// <summary>
    ///     Reads a tarray from the stream and returns it as a List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializer"></param>
    /// <param name="stream"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static List<T> ReadTArrayToList<T>(this IObjectSerializer<T> serializer, IUnrealPackageStream stream, int? size = null) where T : new()
    {
        size ??= stream.ReadInt32();
        var result = new List<T>
        {
            Capacity = size.Value
        };

        for (var i = 0; i < size; i++)
        {
            var obj = new T();
            serializer.DeserializeObject(obj, stream);
            result.Add(obj);
        }

        return result;
    }

    /// <summary>
    ///     Read a array with serialized element size
    /// </summary>
    /// <param name="serializer"></param>
    /// <param name="stream"></param>
    /// <param name="size"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static TArray<T> ReadTArrayWithElementSize<T>(this IStreamSerializer<T> serializer, Stream stream, int? size = null)
    {
        var elementSize = stream.ReadInt32();
        size ??= stream.ReadInt32();
        var result = new TArray<T>
        {
            Capacity = size.Value,
            ElementSize = elementSize
        };

        for (var i = 0; i < size; i++)
        {
            result.Add(serializer.Deserialize(stream));
        }

        return result;
    }

    /// <summary>
    ///     Reads a tarray from the stream and adds it to the output list. The output list is cleared before adding values
    ///     ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializer"></param>
    /// <param name="stream"></param>
    /// <param name="output"></param>
    /// <param name="size"></param>
    public static void ReadTArrayToList<T>(this IStreamSerializer<T> serializer, Stream stream, List<T> output, int? size = null)
    {
        output.Clear();
        size ??= stream.ReadInt32();
        output.EnsureCapacity(size.Value);
        for (var i = 0; i < size; i++)
        {
            output.Add(serializer.Deserialize(stream));
        }
    }


    /// <summary>
    ///     Reads a tarray from the stream and adds it to the output list. The output list is cleared before adding values
    ///     ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializer"></param>
    /// <param name="stream"></param>
    /// <param name="output"></param>
    /// <param name="size"></param>
    public static void ReadTArrayToList<T>(this IObjectSerializer<T> serializer, IUnrealPackageStream stream, List<T> output, int? size = null) where T : new()
    {
        output.Clear();
        size ??= stream.ReadInt32();
        output.EnsureCapacity(size.Value);
        for (var i = 0; i < size; i++)
        {
            var obj = new T();
            serializer.DeserializeObject(obj, stream);
            output.Add(obj);
        }
    }


    /// <summary>
    ///     Writes a array of values to the stream using a given typed serializer. The output can be prepended with the amount
    ///     of elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="serializer"></param>
    /// <param name="values"></param>
    /// <param name="sizeOtion"></param>
    public static void WriteTArray<T>(this IStreamSerializer<T> serializer, Stream stream, T[] values,
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

    /// <summary>
    ///     Writes a array of values to the stream using a given typed serializer. The output can be prepended with the amount
    ///     of elements
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <param name="serializer"></param>
    /// <param name="values"></param>
    public static void BulkWriteTArray<T>(this IStreamSerializer<T> serializer, Stream stream, TArray<T> values)
    {
        stream.WriteInt32(values.ElementSize);
        stream.WriteInt32(values.Count);

        foreach (var value in values)
        {
            serializer.Serialize(stream, value);
        }
    }
}