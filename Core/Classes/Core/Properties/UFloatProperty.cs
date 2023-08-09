using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     Property for a float value
/// </summary>
[NativeOnlyClass("Core", "FloatProperty", typeof(UProperty))]
public class UFloatProperty : UProperty
{
    /// <inheritdoc />
    public UFloatProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        return objStream.ReadSingle();
    }

    /// <inheritdoc />
    public override void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        if (valueObject is not float value)
        {
            return;
        }

        objectStream.WriteSingle(value);
    }

    /// <inheritdoc />
    public override FProperty CreateFProperty(object? value)
    {
        return new FProperty
        {
            Value = value as float?,
            uProperty = this,
            Size = 4,
            Type = PropertyType.FloatProperty,
            Name = Name
        };
    }
}