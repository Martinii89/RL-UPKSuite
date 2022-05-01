using Core.Types;

namespace Core.Classes.Core;

[NativeOnlyClass("Core", "Class", "State")]
public class UClass : UObject /* TODO: wrong base! */
{
    public UClass(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UClass? superClass = null) : base(name, @class, outer,
        ownerPackage)
    {
        SuperClass = superClass;
    }

    public static UClass? StaticClass { get; set; }

    public UClass? SuperClass { get; init; }
}