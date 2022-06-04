using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "MaterialInstance", typeof(UMaterialInterface))]
public class UMaterialInstance : UMaterialInterface
{
    public UMaterialInstance(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }
}