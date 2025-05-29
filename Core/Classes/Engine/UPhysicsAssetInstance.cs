using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine;

[NativeOnlyClass("Engine", "PhysicsAssetInstance", typeof(UObject))]
public class UPhysicsAssetInstance : UObject
{
    public UPhysicsAssetInstance(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }

    public Dictionary<FRigidBodyIndexPair, bool> CollisionDisableTable { get; set; } = new();
}