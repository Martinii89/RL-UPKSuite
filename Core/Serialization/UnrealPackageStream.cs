using System.Text;
using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization;

public class UnrealPackageStream : IUnrealPackageStream
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

    public void WriteFName(string name)
    {
        var fname = _unrealPackage.GetFName(name);
        _nameSerializer.Serialize(BaseStream, fname);
    }

    public string ReadFNameStr()
    {
        return _unrealPackage.GetName(_nameSerializer.Deserialize(BaseStream));
    }

    public bool ReadBool()
    {
        return BaseStream.ReadInt32() == 1;
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
        var length = ReadInt32();
        if (length == 0)
        {
            return string.Empty;
        }

        var stringBytes = ReadBytes(length - 1);
        BaseStream.Move(1); //skip the zero termination
        return Encoding.UTF8.GetString(stringBytes);
    }

    public void WriteFString(string value)
    {
        BaseStream.WriteFString(value);
    }

    public List<T> ReadTArray<T>(Func<IUnrealPackageStream, T> readFunc)
    {
        var res = new List<T>();
        var count = ReadInt32();
        for (var i = 0; i < count; i++)
        {
            res.Add(readFunc(this));
        }

        return res;
    }

    public void WriteTArray<T>(List<T> values, Action<IUnrealPackageStream, T> writeFunc)
    {
        WriteInt32(values.Count);
        foreach (var value in values)
        {
            writeFunc(this, value);
        }
    }

    public void BulkWriteTArray<T>(TArray<T> values, Action<IUnrealPackageStream, T> writeFunc)
    {
        WriteInt32(values.ElementSize);
        WriteTArray(values, writeFunc);
    }

    public Dictionary<TKey, TVal> ReadDictionary<TKey, TVal>(Func<IUnrealPackageStream, TKey?> keyRead, Func<IUnrealPackageStream, TVal> valRead)
        where TKey : notnull
    {
        var res = new Dictionary<TKey, TVal>();

        var mapCount = ReadInt32();

        for (var i = 0; i < mapCount; i++)
        {
            var key = keyRead(this);
            var value = valRead(this);
            if (key is not null)
            {
                res.Add(key, value);
            }
        }

        return res;
    }

    public void WriteDictionary<TKey, TVal>(Dictionary<TKey, TVal> dictionary, Action<IUnrealPackageStream, TKey?> keyWrite,
        Action<IUnrealPackageStream, TVal> valWrite) where TKey : notnull
    {
        WriteInt32(dictionary.Count);
        foreach (var (key, value) in dictionary)
        {
            keyWrite(this, key);
            valWrite(this, value);
        }
    }

    public TMultiMap<TKey, TVal> ReadTMap<TKey, TVal>(Func<IUnrealPackageStream, TKey> keyRead, Func<IUnrealPackageStream, TVal> valRead) where TKey : notnull
    {
        var res = new TMultiMap<TKey, TVal>();

        var mapCount = ReadInt32();

        for (var i = 0; i < mapCount; i++)
        {
            res.Add(keyRead(this), valRead(this));
        }

        return res;
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

    public TArray<T> BulkReadTArray<T>(Func<IUnrealPackageStream, T> readFunc)
    {
        var res = new TArray<T>
        {
            ElementSize = ReadInt32()
        };
        var count = ReadInt32();
        for (var i = 0; i < count; i++)
        {
            res.Add(readFunc(this));
        }

        return res;
    }

    /// <inheritdoc />
    public void WriteInt32(int value)
    {
        BaseStream.WriteInt32(value);
    }

    /// <inheritdoc />
    public void WriteObject(UObject? obj)
    {
        if (obj == null)
        {
            _objectIndexSerializer.Serialize(BaseStream, new ObjectIndex());
            return;
        }

        if (obj.ExportTableItem is not null)
        {
            var exportIndex = obj.OwnerPackage.ExportTable.FindIndex(o => o.Object == obj);
            if (exportIndex != -1)
            {
                _objectIndexSerializer.Serialize(BaseStream, new ObjectIndex(ObjectIndex.FromExportIndex(exportIndex)));
                return;
            }
        }
        else
        {
            var importIndex = obj.OwnerPackage.ImportTable.FindIndex(o => o.ImportedObject == obj);
            if (importIndex != -1)
            {
                _objectIndexSerializer.Serialize(BaseStream, new ObjectIndex(ObjectIndex.FromImportIndex(importIndex)));
                return;
            }
        }

        _objectIndexSerializer.Serialize(BaseStream, new ObjectIndex());
    }

    /// <inheritdoc />
    public void WriteByte(byte value)
    {
        BaseStream.WriteByte(value);
    }

    public void WriteUInt32(uint value)
    {
        BaseStream.WriteUInt32(value);
    }

    public void WriteBool(bool value)
    {
        WriteInt32(value ? 1 : 0);
    }

    public void WriteSingle(float value)
    {
        BaseStream.WriteSingle(value);
    }

    public void WriteBytes(byte[] bytes)
    {
        BaseStream.WriteBytes(bytes);
    }
}