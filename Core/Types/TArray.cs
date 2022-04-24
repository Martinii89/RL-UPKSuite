using System.Data;
using System.Diagnostics;
using Core.Serialization;
using Syroot.BinaryData;

namespace Core.Types;


/// <summary>
/// List wrapper with Unreal array serialization methods
/// </summary>
/// <typeparam name="T"></typeparam>
public class TArray<T> : List<T> where T : new()
{
    private readonly Func<T>? _constructor;

    /// <summary>
    /// Constructs a empty array
    /// </summary>
    public TArray()
    {
    }

    /// <summary>
    /// Constructs a empty array with a element constructor used when deserializing the elements
    /// </summary>
    /// <param name="inConstructor">Element constructor</param>
    public TArray(Func<T> inConstructor)
    {
        _constructor = inConstructor;
    }

    /// <summary>
    /// Deserialize the array from the stream. Starting with the element count, and then the elements. 
    /// </summary>
    /// <param name="reader"></param>
    /// <exception cref="NotImplementedException">Thrown for unsupported elements</exception>
    public void Deserialize(BinaryReader reader)
    {
        var length = reader.ReadInt32();
        Debug.Assert(length >= 0);
        Clear();
        Capacity = length;

        for (var i = 0; i < length; i++)
        {
            var elem = _constructor != null ? _constructor() : new T();
            Debug.Assert(elem != null, nameof(elem) + " != null");
            elem = (T) GenericSerializer.Deserialize(elem, reader);
            Add(elem);
        }
    }

    public void Serialize(Stream stream)
    {
        stream.WriteInt32(Count);
        foreach (var elem in this)
        {
            GenericSerializer.Serialize(elem, stream);
        }
    }
}