using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "InterfaceProperty", "Property")]
public class UInterfaceProperty : UProperty
{
    public UInterfaceProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name,
        @class, outer,
        ownerPackage, objectArchetype)
    {
    }
}