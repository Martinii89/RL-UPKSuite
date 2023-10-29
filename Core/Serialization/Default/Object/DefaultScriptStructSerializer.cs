using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object;

public class DefaultScriptStructSerializer : BaseObjectSerializer<UScriptStruct>
{
    private readonly IObjectSerializer<UStruct> _structSerializer;

    public DefaultScriptStructSerializer(IObjectSerializer<UStruct> structSerializer)
    {
        _structSerializer = structSerializer;
    }

    public override void DeserializeObject(UScriptStruct obj, IUnrealPackageStream objectStream)
    {
        _structSerializer.DeserializeObject(obj, objectStream);

        obj.StructFlags = objectStream.ReadInt32();

        obj.ScriptProperties.AddRange(GetScriptProperties(obj, objectStream));
    }

    private IEnumerable<FProperty> GetScriptProperties(UObject uObject, IUnrealPackageStream objectStream)
    {
        while (true)
        {
            var name = objectStream.ReadFNameStr();

            if (name == "None")
            {
                yield break;
            }

            var typeFName = objectStream.ReadFNameStr();
            var propType = Enum.Parse<PropertyType>(typeFName);

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
                    objectStream.BaseStream.Move(1);
                    break;
                case PropertyType.StructProperty:
                    property.StructName = objectStream.ReadFNameStr();
                    break;
                case PropertyType.ByteProperty:
                    property.EnumName = objectStream.ReadFNameStr();
                    break;
            }

            property.ValueStart = objectStream.BaseStream.Position;
            objectStream.BaseStream.Move(property.Size);


            yield return property;
        }
    }

    public override void SerializeObject(UScriptStruct obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}