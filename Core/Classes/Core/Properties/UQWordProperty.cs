using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A property for a 64bit int value
/// </summary>
[NativeOnlyClass("Core", "QWordProperty", typeof(UProperty))]
public class UQWordProperty : UProperty
{
    /// <inheritdoc />
    public UQWordProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}