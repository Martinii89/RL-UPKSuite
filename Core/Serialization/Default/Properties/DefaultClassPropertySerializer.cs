using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Properties;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Serialization.Default.Properties;

public class DefaultClassPropertySerializer : BaseObjectSerializer<UClassProperty>
{
    private readonly IObjectSerializer<UObjectProperty> _objectPropertySerializer;

    public DefaultClassPropertySerializer(IObjectSerializer<UObjectProperty> objectPropertySerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _objectPropertySerializer = objectPropertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UClassProperty obj, IUnrealPackageStream objectStream)
    {
        _objectPropertySerializer.DeserializeObject(obj, objectStream);

        obj.MetaClass = objectStream.ReadObject() as UClass;
    }

    /// <inheritdoc />
    public override void SerializeObject(UClassProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}