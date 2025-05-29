using RlUpk.Core.Classes.Core;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "SpeedTreeComponent", typeof(UPrimitiveComponent))]
public class USpeedTreeComponent : UPrimitiveComponent
{
    public USpeedTreeComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }
}