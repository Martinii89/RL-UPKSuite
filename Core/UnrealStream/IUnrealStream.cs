using Syroot.BinaryData;

namespace Core.UnrealStream;

public static class StreamExtensions
{
    public static string ReadFString(this Stream stream)
    {
        var length = stream.ReadInt32();
        var fstring = stream.ReadString(length-1);
        stream.Move(1); //skip the zero termination
        return fstring;
    }

    public static void WriteFString(this Stream stream, string value)
    {
        stream.WriteInt32(value.Length+1);
        stream.WriteString(value, StringCoding.ZeroTerminated);
    }
}