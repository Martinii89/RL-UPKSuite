using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "SpotLightComponent", typeof(UPointLightComponent))]
public class USpotLightComponent : UPointLightComponent
{
    public USpotLightComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }
}