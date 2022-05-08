using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A property for a FName value
/// </summary>
[NativeOnlyClass("Core", "NameProperty", "Property")]
public class UNameProperty : UProperty
{
    /// <inheritdoc />
    public UNameProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}