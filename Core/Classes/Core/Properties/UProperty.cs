using Core.Flags;
using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     The base class of all unreal script object properties
/// </summary>
[NativeOnlyClass("Core", "Property", typeof(UField))]
public class UProperty : UField
{
    /// <summary>
    ///     Constructs the base of a script property
    /// </summary>
    /// <param name="name">The name of the property</param>
    /// <param name="class">The class of the property</param>
    /// <param name="outer">The object that has this property</param>
    /// <param name="ownerPackage">The package where this property object is defined</param>
    /// <param name="objectArchetype">I don't think properties can have archetypes? Probably always null?</param>
    public UProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class, outer,
        ownerPackage, objectArchetype)
    {
    }

    public int ArrayDim { get; set; }
    public ulong PropertyFlags { get; set; }
    public string Category { get; set; } = string.Empty;
    public UEnum? ArraySizeEnum { get; set; }
    public ushort RepOffset { get; set; }

    /// <summary>
    ///     Check if the property has the given property flag
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public bool HasPropertyFlag(PropertyFlagsLO flag)
    {
        return ((uint) (PropertyFlags & 0x00000000FFFFFFFFU) & (uint) flag) != 0;
    }
}