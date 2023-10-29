using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A property for a FName value
/// </summary>
[NativeOnlyClass("Core", "NameProperty", typeof(UProperty))]
public class UNameProperty : UProperty
{
    /// <inheritdoc />
    public UNameProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        return objStream.ReadFNameStr();
    }

    public override void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        if (valueObject is not string value)
        {
            return;
        }

        objectStream.WriteFName(value);
    }

    /// <inheritdoc />
    public override FProperty CreateFProperty(object? value)
    {
        return new FProperty
        {
            Value = value as string,
            uProperty = this,
            Size = 8,
            Type = PropertyType.NameProperty,
            Name = Name
        };
    }
}