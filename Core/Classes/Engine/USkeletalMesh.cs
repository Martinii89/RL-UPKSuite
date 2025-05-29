using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "SkeletalMesh", typeof(UObject))]
public class USkeletalMesh : UObject
{
    public USkeletalMesh(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public List<FStaticLodModel> LODModels { get; set; } = new();
    public FBoxSphereBounds BoxSphereBounds { get; set; } = new();
    public List<UMaterialInterface?> Materials { get; set; } = new();
    public FVector Origin { get; set; } = new();
    public FRotator RotOrigin { get; set; } = new();
    public List<FMeshBone> RefSkeleton { get; set; } = new();
    public int SkeletalDepth { get; set; }
    public Dictionary<string, int> NameMap { get; set; } = new();
    public List<UObject?> ClothingAssets { get; set; } = new();
    public List<int> CachedStreamingTextureFactors { get; set; } = new();
    public int PerPolyBoneKDOPsCount { get; set; }
    public int BoneBreakNamesCount { get; set; }
    public int BoneBreakOptionsCount { get; set; }
}