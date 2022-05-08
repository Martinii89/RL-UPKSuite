using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

/// <summary>
///     A UPackage object is a organizing object that holds a group of other objects.
/// </summary>
[NativeOnlyClass("Core", "Package", "Object")]
public class UPackage : UObject
{
    /// <inheritdoc />
    public UPackage(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}