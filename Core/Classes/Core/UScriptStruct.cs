using RlUpk.Core.Flags;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core;

/// <summary>
///     A struct definition for unreal script
/// </summary>
[NativeOnlyClass("Core", "ScriptStruct", typeof(UStruct))]
public class UScriptStruct : UStruct
{
    /// <inheritdoc />
    public UScriptStruct(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }

    public int StructFlags { get; set; }

    /// <summary>
    ///     Check if the struct has the given flag
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public bool HasFlag(StructFlag flag)
    {
        return ((StructFlag) StructFlags).HasFlag(flag);
    }
}