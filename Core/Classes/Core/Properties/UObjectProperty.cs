using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "ObjectProperty", "Property")]
public class UObjectProperty : UProperty
{
    public UObjectProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}