using RlUpk.Core.Classes.Core;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "MaterialInstance", typeof(UMaterialInterface))]
public class UMaterialInstance : UMaterialInterface
{
    public UMaterialInstance(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }

    public List<FMaterialResource> StaticPermutationResource { get; set; } = new();
    public List<FStaticParameterSet> StaticParameters { get; set; } = new();
    public int ResourceCountFlag { get; set; }
}