using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core;

/// <summary>
///     A UPackage object is a organizing object that holds a group of other objects.
/// </summary>
[NativeOnlyClass("Core", "Package", typeof(UObject))]
public class UPackage : UObject
{
    /// <inheritdoc />
    public UPackage(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}