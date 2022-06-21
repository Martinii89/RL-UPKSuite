using Core.Flags;
using Core.Serialization.Abstraction;
using Core.Serialization.Default.Object;
using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     Property for a UStruct value
/// </summary>
[NativeOnlyClass("Core", "StructProperty", typeof(UProperty))]
public class UStructProperty : UProperty
{
    /// <inheritdoc />
    public UStructProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    /// <summary>
    ///     The type of struct this property points to
    /// </summary>
    public UScriptStruct? Struct { get; set; }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        ArgumentNullException.ThrowIfNull(Struct);
        Struct.Deserialize();
        var structValues = new Dictionary<string, object?>();
        if (Struct.HasFlag(StructFlag.Immutable))
        {
            var structProperties = Struct.GetPropertyIteratorIncludingSupers();

            foreach (var structProperty in structProperties)
            {
                //TODO: !! Bad property size !!
                structValues[structProperty.Name] = structProperty.DeserializeValue(obj, objStream, propertySize);
            }

            return structValues;
        }

        var scriptPropertiesSerializer = new ScriptPropertiesSerializer();
        var props = scriptPropertiesSerializer.GetScriptProperties(obj, objStream, Struct);
        foreach (var prop in props)
        {
            structValues[prop.Name] = prop.Value;
        }

        return structValues;
    }
}