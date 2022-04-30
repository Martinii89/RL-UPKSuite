namespace Core.Types;

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

    public UObject? Outer { get; init; }
    private FName _name { get; }

    public string Name => OwnerPackage.GetName(_name);

    public UClass? Class { get; set; }

    public UnrealPackage OwnerPackage { get; init; }

    public UObject? ObjectArchetype { get; init; }
}

public class UClass : UObject /* TODO: wrong base! */
{
    public UClass(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UClass? superClass = null) : base(name, @class, outer,
        ownerPackage)
    {
        SuperClass = superClass;
    }

    public UClass? SuperClass { get; init; }
}