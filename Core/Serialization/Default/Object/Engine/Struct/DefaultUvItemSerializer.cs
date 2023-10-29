using Core.Classes.Engine.Structs;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultUvItemSerializer : BaseObjectSerializer<UvItem>
{
    /// <inheritdoc />
    public override void DeserializeObject(UvItem obj, IUnrealPackageStream objectStream)
    {
        obj.N0 = objectStream.ReadUInt32();
        obj.N1 = objectStream.ReadUInt32();
        if (obj.Uv is not null)
        {
            for (var index = 0; index < obj.Uv.Length; index++)
            {
                obj.Uv[index] = new UvHalf
                {
                    A = objectStream.ReadUInt16(),
                    B = objectStream.ReadUInt16()
                };
            }
        }
        else if (obj.UvFull is not null)
        {
            for (var index = 0; index < obj.UvFull.Length; index++)
            {
                obj.UvFull[index] = new UvFull
                {
                    A = objectStream.ReadSingle(),
                    B = objectStream.ReadSingle()
                };
            }
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UvItem obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}