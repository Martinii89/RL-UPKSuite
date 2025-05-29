using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core.Properties;

/// <summary>
///     A Property for a FString value.
/// </summary>
[NativeOnlyClass("Core", "StrProperty", typeof(UProperty))]
public class UStrProperty : UProperty
{
    /// <inheritdoc />
    public UStrProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        return objStream.ReadFString();
    }

    public override void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        if (valueObject is not string value)
        {
            return;
        }

        objectStream.WriteFString(value);
    }
}