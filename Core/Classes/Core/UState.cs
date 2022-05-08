using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     A unreal script state
/// </summary>
[NativeOnlyClass("Core", "State", "Struct")]
public class UState : UStruct
{
    /// <inheritdoc />
    public UState(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}