using Core.Types;

namespace Core.Classes.Core;

[NativeOnlyClass("Engine", "ActorComponent", typeof(UComponent))]
public class UActorComponent : UComponent
{
    public UActorComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }
}