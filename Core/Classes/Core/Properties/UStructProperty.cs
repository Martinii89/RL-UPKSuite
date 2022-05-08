using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     Property for a UStruct value
/// </summary>
[NativeOnlyClass("Core", "StructProperty", "Property")]
public class UStructProperty : UProperty
{
    /// <inheritdoc />
    public UStructProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}