using System.Diagnostics;
using Core.Serialization.Abstraction;
using Core.Types;

namespace Core.Classes.Core.Properties;

/// <summary>
///     Property representing an array of values
/// </summary>
[NativeOnlyClass("Core", "ArrayProperty", typeof(UProperty))]
public class UArrayProperty : UProperty
{
    /// <inheritdoc />
    public UArrayProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    public UProperty? InnerProperty { get; set; }

    /// <inheritdoc />
    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        //objStream.Move(propertySize);
        var position = objStream.BaseStream.Position;

        var result = new List<object?>();
        var arrayCount = objStream.ReadInt32();
        if (arrayCount == 0 || InnerProperty is null)
        {
            objStream.BaseStream.Move(propertySize - 4);
            return result;
        }

        if (InnerProperty is UStructProperty structProperty)
        {
            structProperty.Deserialize();
            structProperty.Struct?.Deserialize();
        }

        if (InnerProperty is UByteProperty byteProperty)
        {
            byteProperty.Deserialize();
        }

        // subtract the size of the count
        var elementSize = (propertySize - 4) / arrayCount;
        for (var i = 0; i < arrayCount; i++)
        {
            result.Add(InnerProperty?.DeserializeValue(obj, objStream, elementSize));
        }

        var newPosition = objStream.BaseStream.Position;
        if (newPosition - position != propertySize)
        {
            Debugger.Break();
        }

        return result;
    }

    /// <inheritdoc />
    public override void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        if (valueObject is not List<object> values)
        {
            return;
        }

        objectStream.WriteInt32(values.Count);
        if (values.Count == 0)
        {
            return;
        }

        var elementSize = (propertySize - 4) / values.Count;
        foreach (var value in values)
        {
            InnerProperty?.SerializeValue(value, uObject, objectStream, elementSize);
        }
    }

    public override FProperty CreateFProperty(object? value)
    {
        if (value is not List<object> valueList)
        {
            throw new InvalidDataException("Value should be of type List<object>");
        }

        ArgumentNullException.ThrowIfNull(InnerProperty);
        if (InnerProperty is UStructProperty structProperty)
        {
            structProperty.Deserialize();
            structProperty.Struct?.Deserialize();
        }

        var fProperty = new FProperty
        {
            Value = value,
            uProperty = this,
            Size = 4, // at least a 0 count value
            Type = PropertyType.ArrayProperty,
            Name = Name
        };

        foreach (var itemValue in valueList)
        {
            var paramFProperty = InnerProperty.CreateFProperty(itemValue);
            fProperty.Size += paramFProperty.Size;
        }


        return fProperty;
    }
}