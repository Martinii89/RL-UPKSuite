using Core.Types;
using Core.Types.FileSummeryInner;

namespace Core.Serialization.Default;

public class FGenerationsSerializer : IStreamSerializerFor<TArray<FGenerationInfo>>
{
    public TArray<FGenerationInfo> Deserialize(Stream stream)
    {
        var arraySize = stream.ReadInt32();
        var generations = new TArray<FGenerationInfo>
        {
            Capacity = arraySize
        };
        for (var i = 0; i < arraySize; i++)
        {
            generations.Add(new FGenerationInfo
            {
                ExportCount = stream.ReadInt32(),
                NameCount = stream.ReadInt32(),
                NetObjectCount = stream.ReadInt32()
            });
        }

        return generations;
    }

    public void Serialize(Stream stream, TArray<FGenerationInfo> value)
    {
        throw new NotImplementedException();
    }
}