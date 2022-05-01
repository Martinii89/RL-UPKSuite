using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "Property", "Field")]
public class UProperty : UField
{
    public UProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}