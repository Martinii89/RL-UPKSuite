using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "ClassProperty", "Property")]
public class UClassProperty : UProperty
{
    public UClassProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}