using Core.UnrealStream;
using Syroot.BinaryData;

namespace Core.Types.PackageTables;

public class ThumbnailTable
{
    public List<ThumbnailTableItem> Thumbnails { get; private set; } = new();


    internal void Deserialize(Stream reader)
    {
        throw new NotImplementedException();
    }

    internal void Serialize(Stream reader)
    {
        throw new NotImplementedException();
    }
}

public class ThumbnailTableItem
{
    public string Name { get; private set; }
    public string Group { get; private set; }
    public int DataOffset { get; private set; }
    public ThumbnailData? ThumbnailData { get; set; }

    public ThumbnailTableItem(string name, string group, int dataOffset)
    {
        Name = name;
        Group = group;
        DataOffset = dataOffset;
    }

    internal void Deserialize(Stream reader)
    {
        throw new NotImplementedException();
    }

    internal void Serialize(Stream stream)
    {
        stream.WriteFString(Name);
        stream.WriteFString(Group);
        stream.WriteInt32(DataOffset);
    }
}

public class ThumbnailData
{
    public int SizeX { get; private set; }
    public int SizeY { get; private set; }
    public int DataSize { get; private set; }
    public byte[] Data { get; private set; }

    public ThumbnailData(int sizeX, int sizeY, byte[] data)
    {
        SizeX = sizeX;
        SizeY = sizeY;
        Data = data;
        DataSize = data?.Length ?? 0;
    }

    internal void Deserialize(Stream reader)
    {
        throw new NotImplementedException();
    }

    internal void Serialize(Stream stream)
    {
        stream.Write(SizeX);
        stream.Write(SizeY);
        stream.Write(DataSize);
        if (DataSize > 0)
        {
            stream.Write(Data, 0, Data.Length);
        }
    }
}