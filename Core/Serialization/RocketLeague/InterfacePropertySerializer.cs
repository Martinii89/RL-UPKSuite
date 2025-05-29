using System.Diagnostics;

using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Properties;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class InterfacePropertySerializer : BaseObjectSerializer<UInterfaceProperty>
{
    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public InterfacePropertySerializer(IObjectSerializer<UProperty> propertySerializer)
    {
        _propertySerializer = propertySerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UInterfaceProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.InterfaceClass = objectStream.ReadObject() as UClass;
        var name = objectStream.ReadFNameStr();
        if (name != "None")
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UInterfaceProperty obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}