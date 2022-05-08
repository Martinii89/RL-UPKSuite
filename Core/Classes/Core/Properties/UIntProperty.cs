using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A property for a int value
/// </summary>
[NativeOnlyClass("Core", "IntProperty", "Property")]
public class UIntProperty : UProperty
{
    /// <inheritdoc />
    public UIntProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}