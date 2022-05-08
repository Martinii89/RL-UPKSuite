using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A Bool property
/// </summary>
[NativeOnlyClass("Core", "BoolProperty", "Property")]
public class UBoolProperty : UProperty
{
    /// <inheritdoc />
    public UBoolProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}