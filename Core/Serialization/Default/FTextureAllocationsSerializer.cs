using Core.Types;
using Core.Types.FileSummeryInner;

namespace Core.Serialization.Default;

public class FTextureAllocationsSerializer : IStreamSerializerFor<TArray<FTextureType>>
{
    private readonly IStreamSerializerFor<TArray<int>> _intArraySerializer;

    public FTextureAllocationsSerializer(IStreamSerializerFor<TArray<int>> intArraySerializer)
    {
        _intArraySerializer = intArraySerializer;
    }

    public TArray<FTextureType> Deserialize(Stream stream)
    {
        var arraySize = stream.ReadInt32();
        var textureTypes = new TArray<FTextureType>
        {
            Capacity = arraySize
        };
        for (var i = 0; i < arraySize; i++)
        {
            textureTypes.Add(new FTextureType
            {
                SizeX = stream.ReadInt32(),
                SizeY = stream.ReadInt32(),
                NumMips = stream.ReadInt32(),
                Format = stream.ReadInt32(),
                TexCreateFlags = stream.ReadInt32(),
                ExportIndices = _intArraySerializer.Deserialize(stream)
            });
        }

        return textureTypes;
    }

    public void Serialize(Stream stream, TArray<FTextureType> value)
    {
        throw new NotImplementedException();
    }
}