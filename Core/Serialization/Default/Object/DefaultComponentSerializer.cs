using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object;

public class DefaultComponentSerializer : BaseObjectSerializer<UComponent>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultComponentSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UComponent obj, IUnrealPackageStream objectStream)
    {
        obj.TemplateOwnerClass = objectStream.ReadObject() as UClass;
        if (obj.IsDefaultObject || obj.GetOuterEnumerable().Any(x => x.IsDefaultObject))
        {
            obj.TemplateName = objectStream.ReadFNameStr();
        }

        _objectSerializer.DeserializeObject(obj, objectStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UComponent obj, IUnrealPackageStream objectStream)
    {
        objectStream.WriteObject(obj.TemplateOwnerClass);
        if (obj.IsDefaultObject || obj.GetOuterEnumerable().Any(x => x.IsDefaultObject))
        {
            objectStream.WriteFName(obj.TemplateName);
        }

        _objectSerializer.SerializeObject(obj, objectStream);
    }
}