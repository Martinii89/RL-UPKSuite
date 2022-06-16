using Core.Serialization.Abstraction;
using Core.Types;

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

    public override object? DeserializeValue(UObject obj, IUnrealPackageStream objStream, int propertySize)
    {
        var delegateObject = objStream.ReadObject();
        var delegateName = objStream.ReadFNameStr();
        return new FScriptDelegate { Object = delegateObject, FunctionName = delegateName };
    }
}

internal class FScriptDelegate
{
    public UObject? Object { get; set; }
    public string FunctionName { get; set; } = string.Empty;
}