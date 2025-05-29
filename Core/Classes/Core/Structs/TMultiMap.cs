namespace RlUpk.Core.Classes.Core.Structs;

/// <summary>
///     Simple and inefficient implementation of a unreal multimap
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class TMultiMap<TKey, TValue> where TKey : notnull

{
    public int Count { get; private set; }
    public Dictionary<TKey, List<TValue>> Data { get; set; } = new();

    public void Add(TKey key, TValue value)
    {
        Count++;
        if (!Data.ContainsKey(key))
        {
            Data.Add(key, new List<TValue>());
        }

        Data[key].Add(value);
    }
}