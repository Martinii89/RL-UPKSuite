using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class RLObjectPropertySerializer : BaseObjectSerializer<UObjectProperty>
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public RLObjectPropertySerializer(IObjectSerializer<UProperty> propertySerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerializer,
        IStreamSerializerFor<FName> fnameSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _fnameSerializer = fnameSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UObjectProperty obj, Stream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.Object = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        var someNameProbablyJustPadding = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
    }

    /// <inheritdoc />
    public override void SerializeObject(UObjectProperty obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}