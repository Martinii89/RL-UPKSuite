using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "LightMapTexture2D", typeof(UTexture2D))]
public class ULightMapTexture2D : UTexture2D
{
    public ULightMapTexture2D(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }
}