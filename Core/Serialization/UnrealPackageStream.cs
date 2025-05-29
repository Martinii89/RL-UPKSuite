using System.Text;

using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Serialization;

public class UnrealPackageStream(
    Stream baseStream,
    IStreamSerializer<ObjectIndex> objectIndexSerializer,
    IStreamSerializer<FName> nameSerializer,
    UnrealPackage unrealPackage)
    : IUnrealPackageStream
{
    protected readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer = objectIndexSerializer;


    /// <inheritdoc />
    public Stream BaseStream { get; set; } = baseStream;

    /// <inheritdoc />
    public UObject? ReadObject()
    {
        return unrealPackage.GetObject(_objectIndexSerializer.Deserialize(BaseStream));
    }

    /// <inheritdoc />
    public FName ReadFName()
    {
        return nameSerializer.Deserialize(BaseStream);
    }

    /// <inheritdoc />
    public void WriteFName(string name)
    {
        var fname = unrealPackage.GetFName(name);
        nameSerializer.Serialize(BaseStream, fname);
    }

    /// <inheritdoc />
    public string ReadFNameStr()
    {
        return unrealPackage.GetName(nameSerializer.Deserialize(BaseStream));
    }

    /// <inheritdoc />
    public bool ReadBool()
    {
        return BaseStream.ReadInt32() == 1;
    }

    /// <inheritdoc />
    public Seek TemporarySeek(long offset = 0, SeekOrigin origin = SeekOrigin.Current)
    {
        return BaseStream.TemporarySeek(offset, origin);
    }

    /// <inheritdoc />
    public uint ReadUInt32()
    {
        return BaseStream.ReadUInt32();
    }

    /// <inheritdoc />
    public int ReadInt32()
    {
        return BaseStream.ReadInt32();
    }

    /// <inheritdoc />
    public ushort ReadUInt16()
    {
        return BaseStream.ReadUInt16();
    }

    /// <inheritdoc />
    public short ReadInt16()
    {
        return BaseStream.ReadInt16();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void WriteFString(string value)
    {
        BaseStream.WriteFString(value);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void WriteTArray<T>(List<T> values, Action<IUnrealPackageStream, T> writeFunc)
    {
        WriteInt32(values.Count);
        foreach (var value in values)
        {
            writeFunc(this, value);
        }
    }

    /// <inheritdoc />
    public void BulkWriteTArray<T>(TArray<T> values, Action<IUnrealPackageStream, T> writeFunc)
    {
        WriteInt32(values.ElementSize);
        WriteTArray(values, writeFunc);
    }

    /// <inheritdoc />
    public Dictionary<TKey, TVal> ReadDictionary<TKey, TVal>(Func<IUnrealPackageStream, TKey?> keyRead,
        Func<IUnrealPackageStream, TVal> valRead)
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

    /// <inheritdoc />
    public void WriteDictionary<TKey, TVal>(Dictionary<TKey, TVal> dictionary,
        Action<IUnrealPackageStream, TKey?> keyWrite,
        Action<IUnrealPackageStream, TVal> valWrite) where TKey : notnull
    {
        WriteInt32(dictionary.Count);
        foreach (var (key, value) in dictionary)
        {
            keyWrite(this, key);
            valWrite(this, value);
        }
    }

    /// <inheritdoc />
    public TMultiMap<TKey, TVal> ReadTMap<TKey, TVal>(Func<IUnrealPackageStream, TKey> keyRead,
        Func<IUnrealPackageStream, TVal> valRead) where TKey : notnull
    {
        var res = new TMultiMap<TKey, TVal>();

        var mapCount = ReadInt32();

        for (var i = 0; i < mapCount; i++)
        {
            res.Add(keyRead(this), valRead(this));
        }

        return res;
    }

    /// <inheritdoc />
    public void WriteTMap<TKey, TVal>(TMultiMap<TKey, TVal> multiMap, Action<IUnrealPackageStream, TKey> keyWrite,
        Action<IUnrealPackageStream, TVal> valWrite)
        where TKey : notnull
    {
        WriteInt32(multiMap.Count);
        foreach ((TKey key, List<TVal> valueList) in multiMap.Data)
        {
            foreach (var val in valueList)
            {
                keyWrite(this, key);
                valWrite(this, val);
            }
        }
    }

    /// <inheritdoc />
    public byte ReadByte()
    {
        return (byte)BaseStream.ReadByte();
    }

    /// <inheritdoc />
    public ulong ReadUInt64()
    {
        return BaseStream.ReadUInt64();
    }

    /// <inheritdoc />
    public long ReadInt64()
    {
        return BaseStream.ReadInt64();
    }

    /// <inheritdoc />
    public float ReadSingle()
    {
        return BaseStream.ReadSingle();
    }

    /// <inheritdoc />
    public byte[] ReadBytes(int count)
    {
        return BaseStream.ReadBytes(count);
    }

    /// <inheritdoc />
    public ushort[] ReadUInt16s(int count)
    {
        return BaseStream.ReadUInt16s(count);
    }

    /// <inheritdoc />
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
    public virtual void WriteObject(UObject? obj)
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

    /// <inheritdoc />
    public void WriteUInt32(uint value)
    {
        BaseStream.WriteUInt32(value);
    }

    /// <inheritdoc />
    public void WriteBool(bool value)
    {
        WriteInt32(value ? 1 : 0);
    }

    /// <inheritdoc />
    public void WriteSingle(float value)
    {
        BaseStream.WriteSingle(value);
    }

    /// <inheritdoc />
    public void WriteBytes(byte[] bytes)
    {
        BaseStream.WriteBytes(bytes);
    }

    /// <inheritdoc />
    public void WriteUInt16(ushort value)
    {
        BaseStream.WriteUInt16(value);
    }

    /// <inheritdoc />
    public void WriteInt16(short value)
    {
        BaseStream.WriteInt16(value);
    }
}