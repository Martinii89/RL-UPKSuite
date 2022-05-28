using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "LightComponent", typeof(UActorComponent))]
public class ULightComponent : UActorComponent
{
    public ULightComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }
}