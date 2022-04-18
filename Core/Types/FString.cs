using System.Text;

namespace Core.Types;

public class FString
{
    public string InnerString { get; private set; } = string.Empty;
    private bool _bIsUnicode;

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

    public void DeserializeUnicode(BinaryReader reader, int length)
    {
        var data = reader.ReadBytes(-length);
        InnerString = Encoding.Unicode.GetString(data, 0, data.Length - 2);
    }

    public void DeserializeAscii(BinaryReader reader, int length)
    {
        var data = reader.ReadBytes(length);
        InnerString = Encoding.ASCII.GetString(data, 0, data.Length - 1);
    }

    public override string ToString()
    {
        return InnerString;
    }
}