using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     The base class of all unreal script object properties
/// </summary>
[NativeOnlyClass("Core", "Property", "Field")]
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
}