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
}