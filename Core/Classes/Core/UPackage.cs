using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

[NativeOnlyClass("Core", "Package", "Object")]
public class UPackage : UObject
{
    public UPackage(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}