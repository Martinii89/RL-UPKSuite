using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultUVStreamSerializer : IStreamSerializerFor<UvStream>
{
    private readonly IObjectSerializer<UvItem> _uvItemSerializer;

    public DefaultUVStreamSerializer(IObjectSerializer<UvItem> uvItemSerializer)
    {
        _uvItemSerializer = uvItemSerializer;
    }

    /// <inheritdoc />
    public UvStream Deserialize(Stream stream)
    {
        var uvStream = new UvStream
        {
            NumTexCords = stream.ReadInt32(),
            ItemSize = stream.ReadInt32(),
            NumVerts = stream.ReadInt32(),
            BUseFullPrecisionUVs = stream.ReadInt32()
        };

        uvStream.UvStreamItems = stream.ReadTarrayWithElementSize(stream1 =>
        {
            var uvItem = new UvItem(new UvHalf[uvStream.NumTexCords]);
            _uvItemSerializer.DeserializeObject(uvItem, stream1);
            return uvItem;
        });

        return uvStream;
    }


    /// <inheritdoc />
    public void Serialize(Stream stream, UvStream value)
    {
        throw new NotImplementedException();
    }
}