using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

[NativeOnlyClass("Core", "State", "Struct")]
public class UState : UStruct
{
    public UState(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}