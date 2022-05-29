using System.Text;
using Core.Types;

namespace Core.Serialization.Extensions;

/// <summary>
///     Extension methods for reading and writing strings from streams in the format of a FString
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    ///     Reads a <see cref="FString" /> formatted string from the stream and converts it to a string.
    /// </summary>
    /// <param name="stream">Source stream</param>
    /// <returns></returns>
    public static string ReadFString(this Stream stream)
    {
        var length = stream.ReadInt32();
        if (length == 0)
        {
            return string.Empty;
        }

        var stringBytes = stream.ReadBytes(length - 1);
        stream.Move(1); //skip the zero termination
        return Encoding.UTF8.GetString(stringBytes);
        //var fstring = stream.ReadString(length - 1);
        //stream.Move(1); //skip the zero termination
        //return fstring;
    }

    /// <summary>
    ///     Write a string to the stream in the binary format of a <see cref="FString" />.
    ///     Prefixed with a length and then the null terminated string.
    /// </summary>
    /// <param name="stream">Destination stream</param>
    /// <param name="value"></param>
    public static void WriteFString(this Stream stream, string value)
    {
        stream.WriteInt32(value.Length + 1);
        stream.WriteString(value, StringCoding.ZeroTerminated);
    }

    /// <summary>
    ///     Generic reader for TMap objects. Requires a provided function to read the key and values
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="keyRead"></param>
    /// <param name="valRead"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TVal"></typeparam>
    /// <returns></returns>
    public static Dictionary<TKey, TVal> ReadDictionary<TKey, TVal>(this Stream stream, Func<Stream, TKey> keyRead, Func<Stream, TVal> valRead)
        where TKey : notnull
    {
        var res = new Dictionary<TKey, TVal>();

        var mapCount = stream.ReadInt32();

        for (var i = 0; i < mapCount; i++)
        {
            res.Add(keyRead(stream), valRead(stream));
        }


        return res;
    }

    /// <summary>
    ///     Read a TArray with a given read function. Useful when you don't wanna create a specialized object reader
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="readFunc"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> ReadTarray<T>(this Stream stream, Func<Stream, T> readFunc)
    {
        var res = new List<T>();
        var count = stream.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            res.Add(readFunc(stream));
        }

        return res;
    }
}