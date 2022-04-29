using Core.Types;
using Core.Types.FileSummeryInner;

namespace Core.Serialization.Default;

/// <inheritdoc />
public class FGenerationsSerializer : IStreamSerializerFor<TArray<FGenerationInfo>>
{
    /// <inheritdoc />
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

    /// <inheritdoc />
    public void Serialize(Stream stream, TArray<FGenerationInfo> value)
    {
        throw new NotImplementedException();
    }
}

/// <inheritdoc />
public class FGenerationInfoSerializer : IStreamSerializerFor<FGenerationInfo>
{
    /// <inheritdoc />
    public FGenerationInfo Deserialize(Stream stream)
    {
        return new FGenerationInfo
        {
            ExportCount = stream.ReadInt32(),
            NameCount = stream.ReadInt32(),
            NetObjectCount = stream.ReadInt32()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FGenerationInfo value)
    {
        throw new NotImplementedException();
    }
}