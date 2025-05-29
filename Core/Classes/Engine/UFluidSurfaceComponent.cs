using RlUpk.Core.Classes.Core;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "FluidSurfaceComponent", typeof(UPrimitiveComponent))]
public class UFluidSurfaceComponent : UPrimitiveComponent
{
    public UFluidSurfaceComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }
}