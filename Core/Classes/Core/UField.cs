using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core;

/// <summary>
///     A base object for all fields on a unreal script object. Holds a reference to the next field on the object
/// </summary>
[NativeOnlyClass("Core", "Field", typeof(UObject))]
public class UField : UObject
{
    /// <inheritdoc />
    public UField(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    /// <summary>
    ///     Reference to the next field
    /// </summary>
    public UObject? Next { get; set; }
}