using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "NavigationPoint", typeof(AActor))]
public class ANavigationPoint : AActor
{
    public ANavigationPoint(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}