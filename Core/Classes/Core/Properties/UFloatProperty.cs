using Core.Serialization;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Classes.Core.Properties;

/// <summary>
///     Property for a float value
/// </summary>
[NativeOnlyClass("Core", "FloatProperty", typeof(UProperty))]
public class UFloatProperty : UProperty
{
    /// <inheritdoc />
    public UFloatProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, Stream objStream, int propertySize, IStreamSerializer<FName> fnameSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        return objStream.ReadSingle();
    }
}