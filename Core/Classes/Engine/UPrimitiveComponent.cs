using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "PrimitiveComponent", typeof(UActorComponent))]
public class UPrimitiveComponent : UActorComponent
{
    public UPrimitiveComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }
}

[NativeOnlyClass("Engine", "SpeedTreeComponent", typeof(UPrimitiveComponent))]
public class USpeedTreeComponent : UPrimitiveComponent
{
    public USpeedTreeComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }
}