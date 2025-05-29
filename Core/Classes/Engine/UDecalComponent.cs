using RlUpk.Core.Classes.Core;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "DecalComponent", typeof(UPrimitiveComponent))]
public class UDecalComponent : UPrimitiveComponent
{
    public UDecalComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }

    public int NumStaticReceivers { get; set; }
}