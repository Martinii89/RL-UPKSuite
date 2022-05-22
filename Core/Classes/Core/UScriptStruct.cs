using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     A struct definition for unreal script
/// </summary>
[NativeOnlyClass("Core", "ScriptStruct", "Struct")]
public class UScriptStruct : UStruct
{
    /// <inheritdoc />
    public UScriptStruct(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }

    public int StructFlags { get; set; }
}