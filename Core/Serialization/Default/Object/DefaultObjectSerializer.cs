using Core.Classes.Core;
using Core.Classes.Core.Properties;
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

        obj.ScriptProperties.AddRange(GetScriptProperties(obj, objectStream));
    }

    private IEnumerable<FProperty> GetScriptProperties(UObject uObject, Stream objectStream)
    {
        while (true)
        {
            var fName = _fnameSerializer.Deserialize(objectStream);
            var name = uObject.OwnerPackage.GetName(fName);

            if (name == "None")
            {
                yield break;
            }

            var typeFName = _fnameSerializer.Deserialize(objectStream);
            var propType = Enum.Parse<PropertyType>(uObject.OwnerPackage.GetName(typeFName));

            FProperty property = new()
            {
                Package = uObject.OwnerPackage,
                Name = name,
                Type = propType,
                Size = objectStream.ReadInt32(),
                ArrayIndex = objectStream.ReadInt32()
            };
            switch (property.Type)
            {
                case PropertyType.BoolProperty:
                    objectStream.Move(1);
                    break;
                case PropertyType.StructProperty:
                    property.StructName = property.Package.GetName(_fnameSerializer.Deserialize(objectStream));
                    break;
                case PropertyType.ByteProperty:
                    property.EnumName = property.Package.GetName(_fnameSerializer.Deserialize(objectStream));
                    break;
            }

            property.ValueStart = objectStream.Position;
            objectStream.Move(property.Size);


            yield return property;
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