using RlUpk.Core.Flags;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core.Properties;

/// <summary>
///     The base class of all unreal script object properties
/// </summary>
[NativeOnlyClass("Core", "Property", typeof(UField))]
public abstract class UProperty : UField
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
    ///     Deserialize the value of this property from the stream
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="objStream"></param>
    /// <param name="propertySize"></param>
    /// <returns></returns>
    public virtual object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        throw new NotImplementedException(Class?.Name);
    }

    /// <summary>
    ///     Check if the property has the given property flag
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public bool HasPropertyFlag(PropertyFlagsLO flag)
    {
        return ((uint) (PropertyFlags & 0x00000000FFFFFFFFU) & (uint) flag) != 0;
    }

    public virtual void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        throw new NotImplementedException(Class?.Name);
    }

    /// <summary>
    ///     Create a serializable FProperty for adding custom script properties to UObjects
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual FProperty CreateFProperty(object? value)
    {
        throw new NotImplementedException(Class?.Name);
    }
}