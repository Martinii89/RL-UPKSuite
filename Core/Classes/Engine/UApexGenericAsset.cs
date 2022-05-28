using Core.Classes.Core;
using Core.Types;

namespace Core.Classes.Engine;

[NativeOnlyClass("Engine", "ApexAsset", typeof(UObject))]
public class UApexAsset : UObject
{
    public UApexAsset(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}

[NativeOnlyClass("Engine", "ApexGenericAsset", typeof(UApexAsset))]
public class UApexGenericAsset : UApexAsset
{
    public UApexGenericAsset(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer, ownerPackage, objectArchetype)
    {
    }
}