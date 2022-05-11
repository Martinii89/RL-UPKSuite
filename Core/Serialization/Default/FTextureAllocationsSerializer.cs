using Core.Types.FileSummeryInner;

namespace Core.Serialization.Default;

/// <inheritdoc />
public class FTextureAllocationsSerializer : IStreamSerializerFor<FTextureType>
{
    private readonly IStreamSerializerFor<int> _intSerializer;

    /// <summary>
    ///     Constructs a FTextureAllocationsSerializer. Requires a int serializers for the exportIndices
    /// </summary>
    /// <param name="intSerializer"></param>
    public FTextureAllocationsSerializer(IStreamSerializerFor<int> intSerializer)
    {
        _intSerializer = intSerializer;
    }

    /// <inheritdoc />
    public FTextureType Deserialize(Stream stream)
    {
        var type = new FTextureType();
        type.SizeX = stream.ReadInt32();
        type.SizeY = stream.ReadInt32();
        type.NumMips = stream.ReadInt32();
        type.Format = stream.ReadInt32();
        type.TexCreateFlags = stream.ReadInt32();
        _intSerializer.ReadTArrayToList(stream, type.ExportIndices);
        return type;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FTextureType value)
    {
        throw new NotImplementedException();
    }
}