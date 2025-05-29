using RlUpk.Core.Classes.Core;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object;

public class DefaultConstSerializer : BaseObjectSerializer<UConst>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;

    public DefaultConstSerializer(IObjectSerializer<UField> fieldSerializer)
    {
        _fieldSerializer = fieldSerializer;
    }

    public override void DeserializeObject(UConst obj, IUnrealPackageStream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);
        obj.Value = objectStream.ReadFString();
    }

    public override void SerializeObject(UConst obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}