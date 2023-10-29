using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class RLObjectPropertySerializer : BaseObjectSerializer<UObjectProperty>
{
    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public RLObjectPropertySerializer(IObjectSerializer<UProperty> propertySerializer)
    {
        _propertySerializer = propertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UObjectProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.PropertyClass = objectStream.ReadObject() as UClass;
        var someNameProbablyJustPadding = objectStream.ReadFNameStr();
    }

    /// <inheritdoc />
    public override void SerializeObject(UObjectProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}