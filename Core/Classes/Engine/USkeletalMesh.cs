using Core.Classes.Core;
using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "SkeletalMesh", typeof(UObject))]
public class USkeletalMesh : UObject
{
    public USkeletalMesh(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public FBoxSphereBounds BoxSphereBounds { get; set; } = new();
    public List<UMaterial?> Materials { get; set; } = new();
    public FVector Origin { get; set; } = new();
    public FRotator RotOrigin { get; set; } = new();
    public List<FMeshBone> RefSkeleton { get; set; } = new();
}