using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Serialization.Default.Object;

public class DefaultScriptStructSerializer : BaseObjectSerializer<UScriptStruct>
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IObjectSerializer<UStruct> _structSerializer;

    public DefaultScriptStructSerializer(IObjectSerializer<UStruct> structSerializer, IStreamSerializerFor<FName> fnameSerializer)
    {
        _structSerializer = structSerializer;
        _fnameSerializer = fnameSerializer;
    }

    public override void DeserializeObject(UScriptStruct obj, Stream objectStream)
    {
        _structSerializer.DeserializeObject(obj, objectStream);

        obj.StructFlags = objectStream.ReadInt32();

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

    public override void SerializeObject(UScriptStruct obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}