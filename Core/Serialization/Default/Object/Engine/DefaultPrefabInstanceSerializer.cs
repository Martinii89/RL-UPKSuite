using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultPrefabInstanceSerializer : BaseObjectSerializer<APrefabInstance>
{
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultPrefabInstanceSerializer(IObjectSerializer<UObject> objectSerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _objectSerializer = objectSerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(APrefabInstance obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        UObject? ReadObj()
        {
            var objectIndex = _objectIndexSerializer.Deserialize(objectStream.BaseStream);
            return obj.OwnerPackage.GetObject(objectIndex);
        }

        obj.ArchetypeToInstanceMap = objectStream.BaseStream.ReadDictionary(_ =>
        {
            var uObject = ReadObj();
            //ArgumentNullException.ThrowIfNull(uObject);
            return uObject;
        }, _ => ReadObj());

        obj.PI_ObjectMap = objectStream.BaseStream.ReadDictionary(stream =>
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