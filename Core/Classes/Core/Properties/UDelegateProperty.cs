using Core.Serialization;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Classes.Core.Properties;

/// <summary>
///     Delegate property
/// </summary>
[NativeOnlyClass("Core", "DelegateProperty", typeof(UProperty))]
public class UDelegateProperty : UProperty
{
    /// <inheritdoc />
    public UDelegateProperty(FName name, UClass? @class, UObject? outer, UnrealPackage ownerPackage, UObject? objectArchetype = null) : base(name, @class,
        outer,
        ownerPackage, objectArchetype)
    {
    }

    public UFunction? FunctionObject { get; set; }
    public UObject? DelegateObject { get; set; }

    //public override object? DeserializeValue(UObject obj, Stream objStream, int propertySize, IStreamSerializerFor<FName> fnameSerializer,
    //    IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    //{
    //    objStream.Move(propertySize);
    //    return null;
    //    //return base.DeserializeValue(obj, objStream, propertySize, fnameSerializer, objectIndexSerializer);
    //}
}