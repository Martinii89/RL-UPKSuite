using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "ArrayProperty", "Property")]
public class UArrayProperty : UProperty
{
    public UArrayProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}