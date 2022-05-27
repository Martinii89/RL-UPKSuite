using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A property for a class reference
/// </summary>
[NativeOnlyClass("Core", "ClassProperty", typeof(UObjectProperty))]
public class UClassProperty : UObjectProperty
{
    /// <inheritdoc />
    public UClassProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    public UClass? MetaClass { get; set; }
}