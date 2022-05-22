using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

public class DefaultComponentSerializer : BaseObjectSerializer<UComponent>
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerialiser;

    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultComponentSerializer(IStreamSerializerFor<ObjectIndex> objectIndexSerialiser, IObjectSerializer<UObject> objectSerializer,
        IStreamSerializerFor<FName> fnameSerializer)
    {
        _objectIndexSerialiser = objectIndexSerialiser;
        _objectSerializer = objectSerializer;
        _fnameSerializer = fnameSerializer;
    }

    public override void DeserializeObject(UComponent obj, Stream objectStream)
    {
        obj.TemplateOwnerClass = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream)) as UClass;
        if (obj.GetOuterEnumerable().Any(x => x.IsDefaultObject))
        {
            obj.TemplateName = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
        }

        _objectSerializer.DeserializeObject(obj, objectStream);
    }

    public override void SerializeObject(UComponent obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}