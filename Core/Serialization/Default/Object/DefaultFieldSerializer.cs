using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

/// <summary>
///     Default implementation of UField serializer
/// </summary>
public class DefaultFieldSerializer : BaseObjectSerializer<UField>
{
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerialiser;

    private readonly IObjectSerializer<UObject> _objectSerializer;

    /// <summary>
    ///     DefaultFieldSerializer constructor taking in the required field serializers
    /// </summary>
    /// <param name="objectSerializer"></param>
    /// <param name="objectIndexSerialiser"></param>
    public DefaultFieldSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<ObjectIndex> objectIndexSerialiser)
    {
        _objectSerializer = objectSerializer;
        _objectIndexSerialiser = objectIndexSerialiser;
    }


    /// <inheritdoc />
    public override void DeserializeObject(UField field, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(field, objectStream);

        field.Next = field.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream.BaseStream));
    }

    /// <inheritdoc />
    public override void SerializeObject(UField obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}