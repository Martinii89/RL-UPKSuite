using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     A unreal script state
/// </summary>
[NativeOnlyClass("Core", "State", typeof(UStruct))]
public class UState : UStruct
{
    /// <inheritdoc />
    public UState(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public uint ProbeMask { get; set; }
    public ushort LabelTableOffset { get; set; }
    public uint StateFlags { get; set; }

    public Dictionary<string, UFunction> FuncMap { get; set; } = new();
}