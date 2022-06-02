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

    /// <inheritdoc />
    public override void DeserializeObject(UComponent obj, Stream objectStream)
    {
        obj.TemplateOwnerClass = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream)) as UClass;
        if (obj.IsDefaultObject || obj.GetOuterEnumerable().Any(x => x.IsDefaultObject))
        {
            var fName = _fnameSerializer.Deserialize(objectStream);
            obj.TemplateName = obj.OwnerPackage.GetName(fName);
        }

        _objectSerializer.DeserializeObject(obj, objectStream);

        //try
        //{
        //}
        //catch (Exception e)
        //{
        //    throw new Exception($"Failed to deserialize a UComponent: {obj}");
        //}
    }

    /// <inheritdoc />
    public override void SerializeObject(UComponent obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}