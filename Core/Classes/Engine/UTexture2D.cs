using Core.Classes.Core;
using Core.Classes.Engine.Structs;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "Texture2D", typeof(UTexture))]
public class UTexture2D : UTexture
{
    public UTexture2D(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public List<Mip> Mips { get; set; } = new();
    public FGuid TextureFileCacheGuid { get; set; } = new();
    public List<Mip> CachedPVRTCMips { get; set; } = new();
    public int CachedFlashMipsMaxResolution { get; set; }
    public List<Mip> CachedATITCMips { get; set; } = new();
    public FByteBulkData CachedFlashMips { get; set; } = new();
    public List<Mip> CachedETCMips { get; set; } = new();
}