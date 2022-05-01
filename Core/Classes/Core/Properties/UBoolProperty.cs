using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "BoolProperty", "Property")]
public class UBoolProperty : UProperty
{
    public UBoolProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}