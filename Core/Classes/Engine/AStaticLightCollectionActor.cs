using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "StaticLightCollectionActor", typeof(ALight))]
public class AStaticLightCollectionActor : ALight
{
    public AStaticLightCollectionActor(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name,
        @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public List<FMatrix> LightComponentMatrixes { get; set; } = new();
}