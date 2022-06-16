using System.Diagnostics;
using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object;

public class ScriptPropertiesSerializer
{
    public IEnumerable<FProperty> GetScriptProperties(UObject obj, IUnrealPackageStream objectStream, UStruct? propSource = null)
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
            var name = objectStream.ReadFNameStr();

            if (name == "None")
            {
                yield break;
            }


            var typeFName = objectStream.ReadFNameStr();
            var propType = Enum.Parse<PropertyType>(typeFName);

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
                    property.StructName = objectStream.ReadFNameStr();
                    break;
                case PropertyType.ByteProperty:
                    property.EnumName = objectStream.ReadFNameStr();
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
                objectStream.BaseStream.Move(property.Type == PropertyType.BoolProperty ? 1 : property.Size);
                continue;
            }

            property.ValueStart = objectStream.BaseStream.Position;
            property.Value = linkedProperty?.DeserializeValue(obj, objectStream, property.Size);


            yield return property;
        }
    }
}