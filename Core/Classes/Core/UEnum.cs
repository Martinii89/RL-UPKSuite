using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

[NativeOnlyClass("Core", "Enum", "Field")]
public class UEnum : UObject
{
    public UEnum(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}