using Core.Classes.Core;
using Core.Classes.Engine.Structs;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "PhysicsAssetInstance", typeof(UObject))]
public class UPhysicsAssetInstance : UObject
{
    public UPhysicsAssetInstance(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }

    public Dictionary<FRigidBodyIndexPair, bool> CollisionDisableTable { get; set; } = new();
}