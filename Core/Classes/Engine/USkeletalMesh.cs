using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "SkeletalMesh", typeof(UObject))]
public class USkeletalMesh : UObject
{
    public USkeletalMesh(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}