using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

public class DefaultClassSerializer : BaseObjectSerializer<UClass>
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UState> _stateSerializer;

    public DefaultClassSerializer(IObjectSerializer<UState> stateSerializer, IStreamSerializerFor<FName> fnameSerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _stateSerializer = stateSerializer;
        _fnameSerializer = fnameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UClass obj, Stream objectStream)
    {
        _stateSerializer.DeserializeObject(obj, objectStream);
        obj.ClassFlags = objectStream.ReadUInt64();
        obj.Within = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.ConfigName = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));

        var componentCount = objectStream.ReadInt32();
        for (var i = 0; i < componentCount; i++)
        {
            var name = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
            if (obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) is UComponent component)
            {
                obj.ComponentsMap.Add(name, component);
            }
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UClass obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}