using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Core.Properties;

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

    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        var delegateObject = objStream.ReadObject();
        var delegateName = objStream.ReadFNameStr();
        return new FScriptDelegate { Object = delegateObject, FunctionName = delegateName };
    }

    public override void SerializeValue(object? valueObject, UObject uObject, IUnrealPackageStream objectStream, int propertySize)
    {
        if (valueObject is not FScriptDelegate value)
        {
            return;
        }

        objectStream.WriteObject(value.Object);
        objectStream.WriteFName(value.FunctionName);
    }
}

internal class FScriptDelegate
{
    public UObject? Object { get; set; }
    public string FunctionName { get; set; } = string.Empty;
}