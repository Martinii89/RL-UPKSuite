﻿using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
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

    public string DllBindNameOrDummy { get; set; }
    public UObject? DefaultObject { get; set; }

    /// <summary>
    ///     The UClass "class". Every UObject that is a class object will have this as their Class member
    /// </summary>
    public static UClass? StaticClass { get; set; }

    /// <summary>
    ///     The base of this class
    /// </summary>
    public UClass? SuperClass { get; init; }

    /// <summary>
    ///     Serializer compatible with instances of this class
    /// </summary>
    public IObjectSerializer? InstanceSerializer { get; set; }

    /// <summary>
    ///     Custom constructor for special native types
    /// </summary>
    public Func<FName, UObject?, UnrealPackage, UObject?, UObject>? InstanceConstructor { get; set; }

    public ulong ClassFlags { get; set; }
    public UObject? Within { get; set; }
    public string ConfigName { get; set; }

    public Dictionary<string, UComponent> ComponentNameToDefaultObjectMap { get; set; } = new();
    public Dictionary<UObject, UProperty> InterfaceMap { get; set; } = new();
    public List<FName> DontSortCategories { get; set; }
    public List<FName> HideCategories { get; set; }
    public List<FName> AutoExpandCategories { get; set; }
    public List<FName> AutoCollapseCategories { get; set; }
    public int ForceScriptOrder { get; set; }
    public List<FName> ClassGroups { get; set; }
    public string NativeClassName { get; set; }

    /// <summary>
    ///     Returns the first instance serializer found. Walking up the SuperClass chain until one is found, or null if no
    ///     SuperClasses has one
    /// </summary>
    /// <returns></returns>
    public IObjectSerializer? GetInstanceSerializer()
    {
        return GetSuperClassIterator().FirstOrDefault(x => x.InstanceSerializer is not null)?.InstanceSerializer;
    }

    /// <summary>
    ///     Returns a Iterator that iterates the class hierarchy. The first returns is itself
    /// </summary>
    /// <returns></returns>
    public IEnumerable<UClass> GetSuperClassIterator()
    {
        yield return this;
        var super = SuperClass;
        while (super is not null)
        {
            yield return super;
            super = super.SuperClass;
        }
    }

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
        var firstClassWithConstructor = GetSuperClassIterator().FirstOrDefault(x => x.InstanceConstructor is not null);
        if (firstClassWithConstructor?.InstanceConstructor is null)
        {
            return new UObject(name, this, outer, ownerPackage, archetype);
        }

        var obj = firstClassWithConstructor.InstanceConstructor(name, outer, ownerPackage, archetype);
        obj.Class = this;
        return obj;
    }
}