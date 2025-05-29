using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core.Properties;

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

    public override void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        if (valueObject is not byte value)
        {
            throw new InvalidDataException();
        }

        objectStream.WriteByte(value);
    }
}