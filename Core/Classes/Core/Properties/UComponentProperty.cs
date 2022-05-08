using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A Component property
/// </summary>
[NativeOnlyClass("Core", "ComponentProperty", "ObjectProperty")]
public class UComponentProperty : UObjectProperty
{
    /// <inheritdoc />
    public UComponentProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}