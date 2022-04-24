using System.Text;
using Core.Serialization;

namespace Core.Types;

/// <summary>
/// Wrapper for strings stored in unreal packages.
/// </summary>
public class FString: IBinaryDeserializableClass
{
    private string InnerString { get; set; } = string.Empty;
    private bool _bIsUnicode;

    /// <summary>
    /// Deserialize from the stream. Reads the string length as a int32 and then the string characters 
    /// </summary>
    /// <param name="reader"></param>
    public void Deserialize(BinaryReader reader)
    {
        var length = reader.ReadInt32();
        _bIsUnicode = length < 0;

        if (length == 0)
        {
            return;
        }

        if (_bIsUnicode)
        {
            DeserializeUnicode(reader, length);
        }
        else
        {
            DeserializeAscii(reader, length);
        }
    }

    private void DeserializeUnicode(BinaryReader reader, int length)
    {
        var data = reader.ReadBytes(-length);
        InnerString = Encoding.Unicode.GetString(data, 0, data.Length - 2);
    }

    private void DeserializeAscii(BinaryReader reader, int length)
    {
        var data = reader.ReadBytes(length);
        InnerString = Encoding.ASCII.GetString(data, 0, data.Length - 1);
    }

    /// <summary>
    /// Returns the deserialized string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return InnerString;
    }
}