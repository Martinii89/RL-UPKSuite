using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core.Properties;

/// <summary>
///     Interface property
/// </summary>
[NativeOnlyClass("Core", "InterfaceProperty", typeof(UProperty))]
public class UInterfaceProperty : UProperty
{
    /// <inheritdoc />
    public UInterfaceProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name,
        @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public UClass? InterfaceClass { get; set; }
}