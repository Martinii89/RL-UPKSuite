using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

[NativeOnlyClass("Core", "TextBuffer", "Object")]
public class UTextBuffer : UObject
{
    public UTextBuffer(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}