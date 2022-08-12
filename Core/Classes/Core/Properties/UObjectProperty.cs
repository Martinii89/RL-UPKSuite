using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A Property for a UObject value
/// </summary>
[NativeOnlyClass("Core", "ObjectProperty", typeof(UProperty))]
public class UObjectProperty : UProperty
{
    /// <inheritdoc />
    public UObjectProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    public UClass? PropertyClass { get; set; }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        var propObj = objStream.ReadObject();
        return propObj;
    }

    /// <inheritdoc />
    public override void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        objectStream.WriteObject(valueObject as UObject);
    }

    public override FProperty CreateFProperty(object? value)
    {
        return new FProperty
        {
            Value = value as UObject,
            uProperty = this,
            Size = 4,
            Type = PropertyType.ObjectProperty,
            Name = Name
        };
    }
}