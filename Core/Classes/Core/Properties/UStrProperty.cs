using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "StrProperty", "Property")]
public class UStrProperty : UProperty
{
    public UStrProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}