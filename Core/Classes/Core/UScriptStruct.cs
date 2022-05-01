using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

[NativeOnlyClass("Core", "ScriptStruct", "Struct")]
public class UScriptStruct : UStruct
{
    public UScriptStruct(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }
}