using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "FloatProperty", "Property")]
public class UFloatProperty : UProperty
{
    public UFloatProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}