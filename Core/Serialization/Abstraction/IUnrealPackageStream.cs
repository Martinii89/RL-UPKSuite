using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Types;

namespace Core.Serialization.Abstraction;

public interface IUnrealPackageStream
{
    public Stream BaseStream { get; set; }
    Seek TemporarySeek(long offset = 0, SeekOrigin origin = SeekOrigin.Current);
    UObject? ReadObject();
    FName ReadFName();
    void WriteFName(string name);
    string ReadFNameStr();
    bool ReadBool();
    byte ReadByte();
    byte[] ReadBytes(int count);
    ushort ReadUInt16();
    short ReadInt16();
    ushort[] ReadUInt16s(int count);
    uint ReadUInt32();
    int ReadInt32();
    ulong ReadUInt64();
    long ReadInt64();
    float ReadSingle();
    string ReadFString();
    void WriteFString(string value);
    List<T> ReadTArray<T>(Func<IUnrealPackageStream, T> readFunc);
    void WriteTArray<T>(List<T> values, Action<IUnrealPackageStream, T> writeFunc);
    TArray<T> BulkReadTArray<T>(Func<IUnrealPackageStream, T> readFunc);
    void BulkWriteTArray<T>(TArray<T> values, Action<IUnrealPackageStream, T> writeFunc);
    Dictionary<TKey, TVal> ReadDictionary<TKey, TVal>(Func<IUnrealPackageStream, TKey?> keyRead, Func<IUnrealPackageStream, TVal> valRead) where TKey : notnull;

    void WriteDictionary<TKey, TVal>(Dictionary<TKey, TVal> dictionary, Action<IUnrealPackageStream, TKey?> keyWrite,
        Action<IUnrealPackageStream, TVal> valWrite) where TKey : notnull;

    TMultiMap<TKey, TVal> ReadTMap<TKey, TVal>(Func<IUnrealPackageStream, TKey> keyRead, Func<IUnrealPackageStream, TVal> valRead) where TKey : notnull;
    void WriteInt32(int value);
    void WriteObject(UObject? obj);
    void WriteByte(byte value);
    void WriteUInt32(uint value);
    void WriteBool(bool value);
    void WriteSingle(float value);
    void WriteBytes(byte[] bytes);
}