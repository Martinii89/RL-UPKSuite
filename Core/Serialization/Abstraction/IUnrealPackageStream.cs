using Core.Classes.Core;
using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Abstraction;

public interface IUnrealPackageStream
{
    public Stream BaseStream { get; set; }
    UObject? ReadObject();
    FName ReadFName();
    string ReadFNameStr();
    Seek TemporarySeek(long offset = 0, SeekOrigin origin = SeekOrigin.Current);
    uint ReadUInt32();
    int ReadInt32();
    ushort ReadUInt16();
    short ReadInt16();
    string ReadFString();
    byte ReadByte();
    ulong ReadUInt64();
    long ReadInt64();
    float ReadSingle();
    byte[] ReadBytes(int count);
    ushort[] ReadUInt16s(int count);
}

internal class UnrealPackageStream : IUnrealPackageStream
{
    private readonly IStreamSerializer<FName> _nameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly UnrealPackage _unrealPackage;

    public UnrealPackageStream(Stream baseStream, IStreamSerializer<ObjectIndex> objectIndexSerializer, IStreamSerializer<FName> nameSerializer,
        UnrealPackage unrealPackage)
    {
        BaseStream = baseStream;
        _objectIndexSerializer = objectIndexSerializer;
        _nameSerializer = nameSerializer;
        _unrealPackage = unrealPackage;
    }


    public Stream BaseStream { get; set; }

    public UObject? ReadObject()
    {
        return _unrealPackage.GetObject(_objectIndexSerializer.Deserialize(BaseStream));
    }

    public FName ReadFName()
    {
        return _nameSerializer.Deserialize(BaseStream);
    }

    public string ReadFNameStr()
    {
        return _unrealPackage.GetName(_nameSerializer.Deserialize(BaseStream));
    }

    public Seek TemporarySeek(long offset = 0, SeekOrigin origin = SeekOrigin.Current)
    {
        return BaseStream.TemporarySeek(offset, origin);
    }

    public uint ReadUInt32()
    {
        return BaseStream.ReadUInt32();
    }

    public int ReadInt32()
    {
        return BaseStream.ReadInt32();
    }

    public ushort ReadUInt16()
    {
        return BaseStream.ReadUInt16();
    }

    public short ReadInt16()
    {
        return BaseStream.ReadInt16();
    }

    public string ReadFString()
    {
        return BaseStream.ReadFString();
    }

    public byte ReadByte()
    {
        return (byte) BaseStream.ReadByte();
    }

    public ulong ReadUInt64()
    {
        return BaseStream.ReadUInt64();
    }

    public long ReadInt64()
    {
        return BaseStream.ReadInt64();
    }

    public float ReadSingle()
    {
        return BaseStream.ReadSingle();
    }

    public byte[] ReadBytes(int count)
    {
        return BaseStream.ReadBytes(count);
    }

    public ushort[] ReadUInt16s(int count)
    {
        return BaseStream.ReadUInt16s(count);
    }
}