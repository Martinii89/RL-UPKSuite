using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "BrushComponent", typeof(UPrimitiveComponent))]
public class UBrushComponent : UPrimitiveComponent
{
    public UBrushComponent(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
        : base(name, @class, outer, ownerPackage, objectArchetype)
    {
    }
}