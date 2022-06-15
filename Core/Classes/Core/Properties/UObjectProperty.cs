using Core.Serialization;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A Property for a UObject value
/// </summary>
[NativeOnlyClass("Core", "ObjectProperty", typeof(UProperty))]
public class UObjectProperty : UProperty
{
    /// <inheritdoc />
    public UObjectProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    public UClass? PropertyClass { get; set; }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, Stream objStream, int propertySize, IStreamSerializer<FName> fnameSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        var propObj = obj.OwnerPackage.GetObject(objectIndexSerializer.Deserialize(objStream));
        return propObj;
    }
}