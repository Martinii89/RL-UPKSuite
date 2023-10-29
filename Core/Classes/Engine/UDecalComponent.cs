using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "DecalComponent", typeof(UPrimitiveComponent))]
public class UDecalComponent : UPrimitiveComponent
{
    public UDecalComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }

    public int NumStaticReceivers { get; set; }
}