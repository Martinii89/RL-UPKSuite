using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "LightMapTexture2D", typeof(UTexture2D))]
public class ULightMapTexture2D : UTexture2D
{
    public enum ELightMapFlags
    {
        None = 0,
        Streamed = 1,
        SimpleLightmap = 2
    }

    public ULightMapTexture2D(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }

    public ELightMapFlags LightMapFlags { get; set; } = ELightMapFlags.None;
}