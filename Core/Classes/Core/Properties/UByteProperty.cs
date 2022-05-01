using Core.Types;

namespace Core.Classes.Core.Properties;

[NativeOnlyClass("Core", "ByteProperty", "Property")]
public class UByteProperty : UProperty
{
    public UByteProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }
}