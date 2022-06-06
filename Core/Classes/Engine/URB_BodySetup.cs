using Core.Classes.Core;
using Core.Classes.Engine.Structs;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "RB_BodySetup", typeof(UKMeshProps))]
public class URB_BodySetup : UKMeshProps
{
    public URB_BodySetup(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public List<FKCachedConvexData> PreCachedPhysData { get; set; } = new();
}