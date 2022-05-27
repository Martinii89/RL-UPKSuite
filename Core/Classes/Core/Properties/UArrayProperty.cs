using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     Property representing an array of values
/// </summary>
[NativeOnlyClass("Core", "ArrayProperty", typeof(UProperty))]
public class UArrayProperty : UProperty
{
    /// <inheritdoc />
    public UArrayProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    public UProperty? InnerProperty { get; set; }
}