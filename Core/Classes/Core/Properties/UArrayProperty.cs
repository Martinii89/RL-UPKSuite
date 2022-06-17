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

        // subtract the size of the count
        var elementSize = (propertySize - 4) / arrayCount;
        for (var i = 0; i < arrayCount; i++)
        {
            result.Add(InnerProperty?.DeserializeValue(obj, objStream, elementSize));
        }

        return result;
    }
}