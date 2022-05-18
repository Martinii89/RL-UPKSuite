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
}