using System.Diagnostics;
using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.Default.Object;

public class ScriptPropertiesSerializer
{
    private readonly IStreamSerializer<FName> _fnameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    public ScriptPropertiesSerializer(IStreamSerializer<FName> fnameSerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _fnameSerializer = fnameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    public IEnumerable<FProperty> GetScriptProperties(UObject obj, Stream objectStream, UStruct? propSource = null)
    {
        if (propSource is null)
        {
            propSource = obj.Class;
        }
        //UStruct? propSource = obj.Class;
        //if (obj is UScriptStruct scriptStruct)
        //{
        //    propSource = scriptStruct;
        //}

        if (propSource is null)
        {
            Debugger.Break();
        }

        while (true)
        {
            var fName = _fnameSerializer.Deserialize(objectStream);
            var name = obj.OwnerPackage.GetName(fName);

            if (name == "None")
            {
                yield break;
            }


            var typeFName = _fnameSerializer.Deserialize(objectStream);
            var propType = Enum.Parse<PropertyType>(obj.OwnerPackage.GetName(typeFName));

            FProperty property = new()
            {
                Package = obj.OwnerPackage,
                Name = name,
                Type = propType,
                Size = objectStream.ReadInt32(),
                ArrayIndex = objectStream.ReadInt32()
            };


            switch (propType)
            {
                case PropertyType.StructProperty:
                    property.StructName = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
                    break;
                case PropertyType.ByteProperty:
                    property.EnumName = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
                    break;
            }

            var linkedProperty = propSource?.GetProperty(name);
            if (linkedProperty is null)
            {
                Debugger.Break();
                propSource?.Deserialize();
                propSource?.InitProperties();
                linkedProperty = propSource?.GetProperty(name);
                propSource?.Outer?.Deserialize();
                propSource?.Outer?.Class?.Deserialize();
                propSource?.Outer?.Class?.InitProperties();
                objectStream.Move(property.Type == PropertyType.BoolProperty ? 1 : property.Size);
                continue;
            }

            property.ValueStart = objectStream.Position;
            property.Value = linkedProperty?.DeserializeValue(obj, objectStream, property.Size, _fnameSerializer, _objectIndexSerializer);


            yield return property;
        }
    }
}