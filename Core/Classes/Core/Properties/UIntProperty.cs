using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A property for a int value
/// </summary>
[NativeOnlyClass("Core", "IntProperty", typeof(UProperty))]
public class UIntProperty : UProperty
{
    /// <inheritdoc />
    public UIntProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        return objStream.ReadInt32();
    }

    /// <inheritdoc />
    public override void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        if (valueObject is not int value)
        {
            return;
        }

        objectStream.WriteInt32(value);
    }

    /// <inheritdoc />
    public override FProperty CreateFProperty(object? value)
    {
        return new FProperty
        {
            Value = value as int?,
            uProperty = this,
            Size = 4,
            Type = PropertyType.IntProperty,
            Name = Name
        };
    }
}