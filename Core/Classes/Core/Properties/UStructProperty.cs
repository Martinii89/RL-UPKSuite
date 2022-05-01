using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "StructProperty", "Property")]
public class UStructProperty : UProperty
{
    public UStructProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}