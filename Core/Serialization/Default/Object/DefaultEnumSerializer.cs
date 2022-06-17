using Core.Classes;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object;

public class DefaultEnumSerializer : BaseObjectSerializer<UEnum>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;

    public DefaultEnumSerializer(IObjectSerializer<UField> fieldSerializer)
    {
        _fieldSerializer = fieldSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UEnum obj, IUnrealPackageStream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);
        var count = objectStream.ReadInt32();
        for (var i = 0; i < count; i++)
        {
            obj.Names.Add(objectStream.ReadFNameStr());
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UEnum obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}