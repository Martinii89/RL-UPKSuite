using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "NameProperty", "Property")]
public class UNameProperty : UProperty
{
    public UNameProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}