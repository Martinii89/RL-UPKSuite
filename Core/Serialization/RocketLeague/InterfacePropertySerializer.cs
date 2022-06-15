using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class InterfacePropertySerializer : BaseObjectSerializer<UInterfaceProperty>
{
    private readonly IStreamSerializer<FName> _fnameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UProperty> _propertySerializer;

    public InterfacePropertySerializer(IObjectSerializer<UProperty> propertySerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer, IStreamSerializer<FName> fnameSerializer)
    {
        _propertySerializer = propertySerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _fnameSerializer = fnameSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UInterfaceProperty obj, IUnrealPackageStream objectStream)
    {
        _propertySerializer.DeserializeObject(obj, objectStream);

        obj.InterfaceClass = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream)) as UClass;
        var dummyFNameMaybe = _fnameSerializer.Deserialize(objectStream.BaseStream);
        var name = obj.OwnerPackage.GetName(dummyFNameMaybe);
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