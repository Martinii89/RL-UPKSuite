using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     A Unreal script function
/// </summary>
[NativeOnlyClass("Core", "Function", "Struct")]
public class UFunction : UStruct
{
    /// <inheritdoc />
    public UFunction(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}