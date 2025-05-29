using RlUpk.Core.Classes.Core;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "DominantSpotLightComponent", typeof(USpotLightComponent))]
public class UDominantSpotLightComponent : USpotLightComponent
{
    public UDominantSpotLightComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }

    public List<ushort> DominantLightShadowMap { get; set; } = new();
}