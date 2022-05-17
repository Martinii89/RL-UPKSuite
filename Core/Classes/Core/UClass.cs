using Core.Types;

namespace Core.Classes.Core;

/// <summary>
///     The Unreal script class object. The type of every UObject is controlled by it's UClass member.
/// </summary>
[NativeOnlyClass("Core", "Class", "State")]
public class UClass : UState
{
    /// <summary>
    ///     Constructs a new unreal script type.
    /// </summary>
    /// <param name="name">The name of the new type</param>
    /// <param name="class">Should always be the UClass.StaticClass</param>
    /// <param name="outer"></param>
    /// <param name="ownerPackage"></param>
    /// <param name="superClass">The base class</param>
    public UClass(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UClass? superClass = null) : base(name, @class, outer,
        ownerPackage)
    {
        SuperClass = superClass;
    }

    /// <summary>
    ///     The UClass "class". Every UObject that is a class object will have this as their Class member
    /// </summary>
    public static UClass? StaticClass { get; set; }

    /// <summary>
    ///     The base of this class
    /// </summary>
    public UClass? SuperClass { get; init; }

    /// <summary>
    ///     Custom constructor for special native types
    /// </summary>
    public Func<FName, UObject?, UnrealPackage, UObject?, UObject>? InstanceConstructor { get; set; }

    /// <summary>
    ///     Constructs a UObject with this as their class.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="outer"></param>
    /// <param name="ownerPackage"></param>
    /// <param name="archetype"></param>
    /// <returns></returns>
    public UObject NewInstance(FName name, UObject? outer, UnrealPackage ownerPackage, UObject? archetype)
    {
        if (InstanceConstructor is null)
        {
            return new UObject(name, this, outer, ownerPackage, archetype);
        }

        var obj = InstanceConstructor(name, outer, ownerPackage, archetype);
        obj.Class = this;
        return obj;
    }
}