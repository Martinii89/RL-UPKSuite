using Core.Flags;
using Core.Serialization;
using Core.Serialization.Default.Object;
using Core.Types;
using Core.Types.PackageTables;

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

    public UScriptStruct? Struct { get; set; }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, Stream objStream, int propertySize, IStreamSerializerFor<FName> fnameSerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        ArgumentNullException.ThrowIfNull(Struct);
        Struct.Deserialize();
        var structValues = new Dictionary<string, object?>();
        if (Struct.HasFlag(StructFlag.Immutable))
        {
            var structProperties = Struct.GetPropertyIterator();

            foreach (var structProperty in structProperties)
            {
                //!! Bad size !!
                structValues[structProperty.Name] = structProperty.DeserializeValue(obj, objStream, propertySize, fnameSerializer, objectIndexSerializer);
            }

            return structValues;
        }

        //objStream.Move(propertySize);
        //return null;

        var scriptPropertiesSerializer = new ScriptPropertiesSerializer(fnameSerializer, objectIndexSerializer);
        var props = scriptPropertiesSerializer.GetScriptProperties(obj, objStream, Struct);
        foreach (var prop in props)
        {
            structValues[prop.Name] = prop.Value;
        }


        //objStream.Move(propertySize);
        return structValues;
    }
}