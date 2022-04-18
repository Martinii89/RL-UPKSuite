using System.Data;
using System.Diagnostics;
using Core.Serialization;

namespace Core.Types;

// List wrapper with Unreal array serialization methods
public class TArray<T> : List<T> where T : new()
{
    private readonly Func<T>? _constructor;

    public TArray()
    {
    }

    public TArray(Func<T> inConstructor)
    {
        _constructor = inConstructor;
    }


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
}