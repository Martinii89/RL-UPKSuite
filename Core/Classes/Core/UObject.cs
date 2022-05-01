using Core.Types;

namespace Core.Classes.Core;

[NativeOnlyClass("Core", "Object")]
public class UObject
{
    public UObject(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null)
    {
        _name = name;
        Class = @class;
        Outer = outer;
        OwnerPackage = ownerPackage;
        ObjectArchetype = objectArchetype;
    }

    public UObject? Outer { get; set; }
    private FName _name { get; }

    public string Name => OwnerPackage.GetName(_name);

    public UClass? Class { get; set; }

    public UnrealPackage OwnerPackage { get; init; }

    public UObject? ObjectArchetype { get; init; }
}