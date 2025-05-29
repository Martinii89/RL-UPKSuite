using System.Diagnostics;

using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Properties;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object;

public class ScriptPropertiesSerializer
{
    public IEnumerable<FProperty> GetScriptProperties(UObject obj, IUnrealPackageStream objectStream, UStruct? propSource = null)
    {
        propSource ??= obj.Class;


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

            if (name.Contains("unnamed"))
            {
                Debugger.Break();
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

            property.uProperty = linkedProperty;
            property.ValueStart = objectStream.BaseStream.Position;
            property.Value = linkedProperty?.DeserializeValue(obj, objectStream, property.Size);


            yield return property;
        }
    }

    public void WriteScriptProperties(IEnumerable<FProperty> properties, UObject obj, IUnrealPackageStream objectStream)
    {
        foreach (var fProperty in properties)
        {
            ArgumentNullException.ThrowIfNull(fProperty.uProperty);
            objectStream.WriteFName(fProperty.Name);
            objectStream.WriteFName(fProperty.Type.ToString());
            objectStream.WriteInt32(fProperty.Size);
            objectStream.WriteInt32(fProperty.ArrayIndex);
            switch (fProperty.Type)
            {
                case PropertyType.StructProperty:
                    objectStream.WriteFName(fProperty.StructName);
                    break;
                case PropertyType.ByteProperty:
                    objectStream.WriteFName(fProperty.EnumName);
                    break;
            }

            fProperty.uProperty.SerializeValue(fProperty.Value, obj, objectStream, fProperty.Size);
        }

        objectStream.WriteFName("None");
    }
}