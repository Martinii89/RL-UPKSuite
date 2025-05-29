using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core;

/// <summary>
///     Represents a enumeration value
/// </summary>
[NativeOnlyClass("Core", "Enum", typeof(UField))]
public class UEnum : UField
{
    /// <inheritdoc />
    public UEnum(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public List<string> Names { get; set; } = new();
}