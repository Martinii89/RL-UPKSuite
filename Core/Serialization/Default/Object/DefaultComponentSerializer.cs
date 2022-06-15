using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

public class DefaultComponentSerializer : BaseObjectSerializer<UComponent>
{
    private readonly IStreamSerializer<FName> _fnameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerialiser;

    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultComponentSerializer(IStreamSerializer<ObjectIndex> objectIndexSerialiser, IObjectSerializer<UObject> objectSerializer,
        IStreamSerializer<FName> fnameSerializer)
    {
        _objectIndexSerialiser = objectIndexSerialiser;
        _objectSerializer = objectSerializer;
        _fnameSerializer = fnameSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UComponent obj, IUnrealPackageStream objectStream)
    {
        obj.TemplateOwnerClass = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream.BaseStream)) as UClass;
        if (obj.IsDefaultObject || obj.GetOuterEnumerable().Any(x => x.IsDefaultObject))
        {
            var fName = _fnameSerializer.Deserialize(objectStream.BaseStream);
            obj.TemplateName = obj.OwnerPackage.GetName(fName);
        }

        _objectSerializer.DeserializeObject(obj, objectStream);
    }

    /// <inheritdoc />
    public override void SerializeObject(UComponent obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}