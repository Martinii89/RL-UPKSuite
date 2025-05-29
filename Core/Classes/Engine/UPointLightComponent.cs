using RlUpk.Core.Classes.Core;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "PointLightComponent", typeof(ULightComponent))]
public class UPointLightComponent : ULightComponent
{
    public UPointLightComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }
}