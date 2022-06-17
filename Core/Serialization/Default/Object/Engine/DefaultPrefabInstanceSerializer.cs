using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultPrefabInstanceSerializer : BaseObjectSerializer<APrefabInstance>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultPrefabInstanceSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(APrefabInstance obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        UObject? ReadObj()
        {
            return objectStream.ReadObject();
        }

        obj.ArchetypeToInstanceMap = objectStream.ReadDictionary(_ =>
        {
            var uObject = ReadObj();
            return uObject;
        }, _ => ReadObj());

        obj.PI_ObjectMap = objectStream.ReadDictionary(stream =>
        {
            var uObject = ReadObj();
            ArgumentNullException.ThrowIfNull(uObject);
            return uObject;
        }, _ => objectStream.ReadInt32());
    }

    /// <inheritdoc />
    public override void SerializeObject(APrefabInstance obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}