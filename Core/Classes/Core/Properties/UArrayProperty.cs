using Core.Serialization;
using Core.Types;
using Core.Types.PackageTables;

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
    public override object? DeserializeValue(UObject obj, Stream objStream, int propertySize, IStreamSerializer<FName> fnameSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        //objStream.Move(propertySize);
        var result = new List<object?>();
        var arrayCount = objStream.ReadInt32();
        if (arrayCount == 0 || InnerProperty is null)
        {
            objStream.Move(propertySize - 4);
            return result;
        }

        if (InnerProperty is UStructProperty structProperty)
        {
            structProperty.Deserialize();
            structProperty.Struct?.Deserialize();
            //Debugger.Break();
            //objStream.Move(propertySize - 4);
            //return result;
        }

        // subtract the size of the count
        var elementSize = (propertySize - 4) / arrayCount;
        for (var i = 0; i < arrayCount; i++)
        {
            result.Add(InnerProperty?.DeserializeValue(obj, objStream, elementSize, fnameSerializer, objectIndexSerializer));
        }

        return result;
    }
}