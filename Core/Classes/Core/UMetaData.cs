using Core.Classes.Core;
using Core.Types;

namespace Core.Classes;

[NativeOnlyClass("Core", "MetaData", "Object")]
public class UMetaData : UObject
{
    public UMetaData(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}