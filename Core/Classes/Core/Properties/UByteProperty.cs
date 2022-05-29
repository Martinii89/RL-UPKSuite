using Core.Serialization;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Classes.Core.Properties;

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
    public override object? DeserializeValue(UObject obj, Stream objStream, int propertySize, IStreamSerializerFor<FName> fnameSerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        if (Enum is null)
        {
            return (byte) objStream.ReadByte();
        }

        return obj.OwnerPackage.GetName(fnameSerializer.Deserialize(objStream));
    }
}