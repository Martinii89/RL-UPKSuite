using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core;

/// <summary>
///     A Component object
/// </summary>
[NativeOnlyClass("Core", "Component", typeof(UObject))]
public class UComponent : UObject
{
    /// <inheritdoc />
    public UComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public UClass? TemplateOwnerClass { get; set; } = null;
    public string TemplateName { get; set; } = string.Empty;
    public bool ExtraFourBytes { get; set; } = false;
}