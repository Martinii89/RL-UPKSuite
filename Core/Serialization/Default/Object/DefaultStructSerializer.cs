using Core.Classes;
using Core.Serialization.Abstraction;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

/// <summary>
///     Default implementation for a UStruct serializer
/// </summary>
public class DefaultStructSerializer : BaseObjectSerializer<UStruct>
{
    private readonly IObjectSerializer<UField> _fieldSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerialiser;

    /// <summary>
    ///     Construct a DefaultStructSerializer with the required field serializers
    /// </summary>
    /// <param name="fieldSerializer"></param>
    /// <param name="objectIndexSerialiser"></param>
    public DefaultStructSerializer(IObjectSerializer<UField> fieldSerializer, IStreamSerializerFor<ObjectIndex> objectIndexSerialiser)
    {
        _fieldSerializer = fieldSerializer;
        _objectIndexSerialiser = objectIndexSerialiser;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UStruct obj, Stream objectStream)
    {
        _fieldSerializer.DeserializeObject(obj, objectStream);

        obj.SuperStruct = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream)) as UStruct;
        obj.ScriptText = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream)) as UTextBuffer;


        obj.Children = obj.OwnerPackage.GetObject(_objectIndexSerialiser.Deserialize(objectStream)) as UField;
    }

    /// <inheritdoc />
    public override void SerializeObject(UStruct obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}