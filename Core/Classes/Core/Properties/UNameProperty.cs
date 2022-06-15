using Core.Serialization;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A property for a FName value
/// </summary>
[NativeOnlyClass("Core", "NameProperty", typeof(UProperty))]
public class UNameProperty : UProperty
{
    /// <inheritdoc />
    public UNameProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, Stream objStream, int propertySize, IStreamSerializer<FName> fnameSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        return obj.OwnerPackage.GetName(fnameSerializer.Deserialize(objStream));
    }
}