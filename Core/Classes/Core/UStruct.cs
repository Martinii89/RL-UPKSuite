using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     Base type for all unreal script objects with fields
/// </summary>
[NativeOnlyClass("Core", "Struct", "Field")]
public class UStruct : UField
{
    /// <inheritdoc />
    public UStruct(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public UField? Children { get; set; }
    public UStruct? SuperStruct { get; set; }
    public UTextBuffer? ScriptText { get; set; }
    public UTextBuffer? CppText { get; set; }
    public int Line { get; set; }
    public int TextPos { get; set; }
    public int ScriptBytecodeSize { get; set; }
    public int DataScriptSize { get; set; }
}