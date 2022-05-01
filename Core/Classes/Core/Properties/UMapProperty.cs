using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "MapProperty", "Property")]
public class UMapProperty : UProperty
{
    public UMapProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}