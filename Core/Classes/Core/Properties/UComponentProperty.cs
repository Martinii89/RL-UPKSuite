using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "ComponentProperty", "Property")]
public class UComponentProperty : UProperty
{
    public UComponentProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}