using Core.Serialization;
using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Classes.Core.Properties;

/// <summary>
///     A Property for a FString value.
/// </summary>
[NativeOnlyClass("Core", "StrProperty", typeof(UProperty))]
public class UStrProperty : UProperty
{
    /// <inheritdoc />
    public UStrProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, Stream objStream, int propertySize, IStreamSerializer<FName> fnameSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        return objStream.ReadFString();
    }
}