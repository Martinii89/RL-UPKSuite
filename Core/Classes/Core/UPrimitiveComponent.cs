using Core.Types;

namespace Core.Classes.Core;

[NativeOnlyClass("Engine", "PrimitiveComponent", typeof(UActorComponent))]
public class UPrimitiveComponent : UActorComponent
{
    public UPrimitiveComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }
}