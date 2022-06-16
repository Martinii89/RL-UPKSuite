using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A Bool property
/// </summary>
[NativeOnlyClass("Core", "BoolProperty", typeof(UProperty))]
public class UBoolProperty : UProperty
{
    /// <inheritdoc />
    public UBoolProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        return objStream.ReadByte();
    }
}