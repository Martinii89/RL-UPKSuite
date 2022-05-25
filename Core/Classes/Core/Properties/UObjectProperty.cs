using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A Property for a UObject value
/// </summary>
[NativeOnlyClass("Core", "ObjectProperty", "Property")]
public class UObjectProperty : UProperty
{
    /// <inheritdoc />
    public UObjectProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    public UObject? Object { get; set; }
}