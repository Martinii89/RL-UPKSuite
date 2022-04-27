using Core.Extensions;
using Core.Types;

namespace Core.Serialization.Default;

public class FStringArraySerializer : IStreamSerializerFor<TArray<FString>>
{
    public TArray<FString> Deserialize(Stream stream)
    {
        var arraySize = stream.ReadInt32();
        var strings = new TArray<FString>
        {
            Capacity = arraySize
        };
        for (var i = 0; i < arraySize; i++)
        {
            strings.Add(new FString
            {
                InnerString = stream.ReadFString()
            });
        }

        return strings;
    }

    public void Serialize(Stream stream, TArray<FString> value)
    {
        throw new NotImplementedException();
    }
}