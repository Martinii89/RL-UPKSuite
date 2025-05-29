using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core.Properties;

/// <summary>
///     A Byte property. Often this will be a Enum.
/// </summary>
[NativeOnlyClass("Core", "ByteProperty", typeof(UProperty))]
public class UByteProperty : UProperty
{
    /// <inheritdoc />
    public UByteProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    public UEnum? Enum { get; set; }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        if (Enum is null)
        {
            return objStream.ReadByte();
        }

        return objStream.ReadFNameStr();
    }

    /// <inheritdoc />
    public override void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        if (Enum is null)
        {
            if (valueObject is not byte byteValue)
            {
                throw new InvalidDataException();
            }

            objectStream.WriteByte(byteValue);
            return;
        }

        if (valueObject is not string stringValue)
        {
            throw new InvalidDataException();
        }

        objectStream.WriteFName(stringValue);
    }

    /// <inheritdoc />
    public override FProperty CreateFProperty(object? value)
    {
        ArgumentNullException.ThrowIfNull(Enum);
        return new FProperty
        {
            Value = value as string,
            uProperty = this,
            Size = 4,
            Type = PropertyType.ByteProperty,
            Name = Name,
            EnumName = Enum.Name
        };
    }
}