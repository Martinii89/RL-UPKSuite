using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     A Component object
/// </summary>
[NativeOnlyClass("Core", "Component", "Object")]
public class UComponent : UObject
{
    /// <inheritdoc />
    public UComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}