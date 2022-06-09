using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultUvItemSerializer : BaseObjectSerializer<UvItem>
{
    /// <inheritdoc />
    public override void DeserializeObject(UvItem obj, Stream objectStream)
    {
        obj.N0 = objectStream.ReadUInt32();
        obj.N1 = objectStream.ReadUInt32();
        for (var index = 0; index < obj.Uv.Length; index++)
        {
            obj.Uv[index] = new UvHalf
            {
                A = objectStream.ReadUInt16(),
                B = objectStream.ReadUInt16()
            };
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UvItem obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}