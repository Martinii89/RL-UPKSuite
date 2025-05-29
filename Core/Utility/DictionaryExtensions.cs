using System.Runtime.InteropServices;

namespace RlUpk.Core.Utility;

public static class DictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, Func<TValue> valueFunc)
        where TKey : notnull
    {
        ref var dictionaryValue = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, key, out var exists);
        if (exists)
            return dictionaryValue!;

        dictionaryValue = valueFunc();
        return dictionaryValue;
    }
}