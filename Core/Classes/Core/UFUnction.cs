using Core.Classes.Core;
using Core.Flags;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     A Unreal script function
/// </summary>
[NativeOnlyClass("Core", "Function", typeof(UStruct))]
public class UFunction : UStruct
{
    /// <inheritdoc />
    public UFunction(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public ushort INative { get; set; }
    public byte OperPrecedence { get; set; }
    public ulong FunctionFlags { get; set; }
    public ushort RepOffset { get; set; }

    public string FriendlyName { get; set; }

    public bool HasFunctionFlag(FunctionFlags flag)
    {
        return ((uint) FunctionFlags & (uint) flag) != 0;
    }
}