using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "IntProperty", "Property")]
public class UIntProperty : UProperty
{
    public UIntProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}