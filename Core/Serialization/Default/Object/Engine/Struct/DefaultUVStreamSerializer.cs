using Core.Classes.Engine.Structs;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultUVStreamSerializer : IStreamSerializer<UvStream>
{
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
            UvItem uvItem;
            if (uvStream.BUseFullPrecisionUVs == 1)
            {
                uvItem = new UvItem(new UvFull[uvStream.NumTexCords]);
                DeserializeUvItem(uvItem, stream);
            }
            else
            {
                uvItem = new UvItem(new UvHalf[uvStream.NumTexCords]);
                DeserializeUvItem(uvItem, stream);
            }

            return uvItem;
        });

        return uvStream;
    }


    /// <inheritdoc />
    public void Serialize(Stream stream, UvStream value)
    {
        throw new NotImplementedException();
    }

    private void DeserializeUvItem(UvItem obj, Stream stream)
    {
        obj.N0 = stream.ReadUInt32();
        obj.N1 = stream.ReadUInt32();
        if (obj.Uv is not null)
        {
            for (var index = 0; index < obj.Uv.Length; index++)
            {
                obj.Uv[index] = DeserializeUvHalf(stream);
            }
        }
        else if (obj.UvFull is not null)
        {
            for (var index = 0; index < obj.UvFull.Length; index++)
            {
                obj.UvFull[index] = DeserializeUvFull(stream);
            }
        }
    }

    private UvFull DeserializeUvFull(Stream stream)
    {
        return new UvFull
        {
            A = stream.ReadSingle(),
            B = stream.ReadSingle()
        };
    }

    private UvHalf DeserializeUvHalf(Stream stream)
    {
        return new UvHalf
        {
            A = stream.ReadUInt16(),
            B = stream.ReadUInt16()
        };
    }
}