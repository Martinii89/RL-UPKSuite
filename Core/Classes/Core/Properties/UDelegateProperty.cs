using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "DelegateProperty", "Property")]
public class UDelegateProperty : UProperty
{
    public UDelegateProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}