using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class RLObjectPropertySerializer : BaseObjectSerializer<UObjectProperty>
{
    private readonly IStreamSerializer<FName> _fnameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public RLObjectPropertySerializer(IObjectSerializer<UProperty> propertySerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer,
        IStreamSerializer<FName> fnameSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _fnameSerializer = fnameSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UObjectProperty obj, Stream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.PropertyClass = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UClass;
        var someNameProbablyJustPadding = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
    }

    /// <inheritdoc />
    public override void SerializeObject(UObjectProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}