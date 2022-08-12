using System.Diagnostics;
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
    private const string FpropertyListKey = "__FPropertyLIST";

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
        var props = scriptPropertiesSerializer.GetScriptProperties(obj, objStream, Struct).ToList();
        foreach (var prop in props)
        {
            structValues[prop.Name] = prop.Value;
        }

        // it's a hack to preserve the FProperties for serialization purposes
        structValues[FpropertyListKey] = props;

        return structValues;
    }

    /// <inheritdoc />
    public override void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        if (valueObject is not Dictionary<string, object?> structValues)
        {
            return;
        }


        ArgumentNullException.ThrowIfNull(Struct);

        if (Struct.HasFlag(StructFlag.Immutable))
        {
            var structProperties = Struct.GetPropertyIteratorIncludingSupers();
            foreach (var structProperty in structProperties)
            {
                if (structValues.ContainsKey(structProperty.Name))
                {
                    structProperty.SerializeValue(structValues[structProperty.Name], uObject, objectStream, propertySize);
                }
                else
                {
                    Debugger.Break();
                }
            }

            return;
        }

        var fPropertyList = structValues[FpropertyListKey] as List<FProperty>;
        var scriptPropertiesSerializer = new ScriptPropertiesSerializer();
        scriptPropertiesSerializer.WriteScriptProperties(fPropertyList, uObject, objectStream);
    }

    public override FProperty CreateFProperty(object? value)
    {
        if (value is not Dictionary<string, object?> structValues)
        {
            throw new InvalidDataException("Value should be of type Dictionary<string, object?>");
        }

        ArgumentNullException.ThrowIfNull(Struct);
        if (Struct.HasFlag(StructFlag.Immutable))
        {
            throw new NotImplementedException();
        }

        var fProperty = new FProperty
        {
            Value = value,
            uProperty = this,
            Size = 8, // at least a None FName
            StructName = Struct.Name,
            Type = PropertyType.StructProperty,
            Name = Name
        };

        var fPropertyList = new List<FProperty>();
        foreach (var (paramName, paramValue) in structValues)
        {
            var parameter = Struct.GetProperty(paramName);
            ArgumentNullException.ThrowIfNull(parameter);
            var paramFProperty = parameter.CreateFProperty(paramValue);
            var additionalSize = 24 + paramFProperty.Size; // 24 is the size of Name\Type\size\array index
            if (paramFProperty.Type is PropertyType.StructProperty or PropertyType.ByteProperty)
            {
                additionalSize += 8;
            }

            fProperty.Size += additionalSize;
            fPropertyList.Add(paramFProperty);
        }

        structValues[FpropertyListKey] = fPropertyList;

        return fProperty;
    }
}