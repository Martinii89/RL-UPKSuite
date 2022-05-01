using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "QWordProperty", "Property")]
public class UQWordProperty : UProperty
{
    public UQWordProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}