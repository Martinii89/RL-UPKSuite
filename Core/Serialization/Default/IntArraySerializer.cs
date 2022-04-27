using Core.Types;

namespace Core.Serialization.Default;

public class IntArraySerializer : IStreamSerializerFor<TArray<int>>
{
    public TArray<int> Deserialize(Stream stream)
    {
        var arraySize = stream.ReadInt32();
        var array = new TArray<int>
        {
            Capacity = arraySize
        };
        for (var i = 0; i < arraySize; i++)
        {
            array.Add(stream.ReadInt32());
        }

        return array;
    }

    public void Serialize(Stream stream, TArray<int> value)
    {
        throw new NotImplementedException();
    }
}