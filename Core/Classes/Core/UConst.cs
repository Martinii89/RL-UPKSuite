using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core;

/// <summary>
///     A unreal script constant value. The constant is represented as a string.
/// </summary>
[NativeOnlyClass("Core", "Const", typeof(UField))]
public class UConst : UField
{
    /// <inheritdoc />
    public UConst(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public string Value { get; set; } = string.Empty;
}