using Core.Serialization;
using Core.Types;
using Core.Types.PackageTables;

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
    public override object? DeserializeValue(UObject obj, Stream objStream, int propertySize, IStreamSerializer<FName> fnameSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        return objStream.ReadByte() == 1;
    }
}