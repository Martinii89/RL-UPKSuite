using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object;

/// <summary>
///     Default implementation for a UObject serializer
/// </summary>
public class DefaultObjectSerializer : BaseObjectSerializer<UObject>
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;

    public DefaultObjectSerializer(IStreamSerializerFor<FName> fnameSerializer)
    {
        _fnameSerializer = fnameSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UObject obj, Stream objectStream)
    {
        obj.NetIndex = objectStream.ReadInt32();

        if (obj.Class == UClass.StaticClass)
        {
            return;
        }

        while (SerializeScriptProperties(obj, objectStream))
        {
        }
    }

    private bool SerializeScriptProperties(UObject uObject, Stream objectStream)
    {
        // skip em all for now
        var fName = _fnameSerializer.Deserialize(objectStream);
        var name = uObject.OwnerPackage.GetName(fName);

        if (name == "None")
        {
            return false;
        }

        var typeFName = _fnameSerializer.Deserialize(objectStream);
        var typeName = uObject.OwnerPackage.GetName(typeFName);
        var size = objectStream.ReadInt32();
        var arrayIndex = objectStream.ReadInt32();
        objectStream.Move(size);

        return true;
    }

    /// <inheritdoc />
    public override void SerializeObject(UObject obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}