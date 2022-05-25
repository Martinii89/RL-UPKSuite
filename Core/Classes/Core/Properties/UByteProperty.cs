using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A Byte property. Often this will be a Enum.
/// </summary>
[NativeOnlyClass("Core", "ByteProperty", "Property")]
public class UByteProperty : UProperty
{
    /// <inheritdoc />
    public UByteProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    public UEnum? Enum { get; set; }
}