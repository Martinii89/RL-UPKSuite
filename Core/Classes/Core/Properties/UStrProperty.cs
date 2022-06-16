using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Classes.Core.Properties;

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
}