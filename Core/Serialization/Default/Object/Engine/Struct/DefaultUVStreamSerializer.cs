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
        stream.WriteInt32(value.NumTexCords);
        stream.WriteInt32(value.ItemSize);
        stream.WriteInt32(value.NumVerts);
        stream.WriteInt32(value.BUseFullPrecisionUVs);

        stream.WriteInt32(value.UvStreamItems.ElementSize);
        stream.WriteInt32(value.UvStreamItems.Count);

        foreach (var uvItem in value.UvStreamItems)
        {
            SerializeUvItem(uvItem, stream);
        }
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

    private void SerializeUvItem(UvItem obj, Stream stream)
    {
        stream.WriteUInt32(obj.N0);
        stream.WriteUInt32(obj.N1);
        if (obj.Uv is not null)
        {
            foreach (var t in obj.Uv)
            {
                SerializeUvHalf(stream, t);
            }
        }
        else if (obj.UvFull is not null)
        {
            foreach (var t in obj.UvFull)
            {
                SerializeUvFull(stream, t);
            }
        }
    }

    private void SerializeUvFull(Stream stream, UvFull uvFull)
    {
        stream.WriteSingle(uvFull.A);
        stream.WriteSingle(uvFull.B);
    }

    private void SerializeUvHalf(Stream stream, UvHalf uvHalf)
    {
        stream.WriteUInt16(uvHalf.A);
        stream.WriteUInt16(uvHalf.B);
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