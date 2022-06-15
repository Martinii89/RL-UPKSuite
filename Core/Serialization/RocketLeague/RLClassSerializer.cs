using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class RLClassSerializer : BaseObjectSerializer<UClass>
{
    private readonly IStreamSerializer<FName> _fnameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UState> _stateSerializer;

    public RLClassSerializer(IObjectSerializer<UState> stateSerializer, IStreamSerializer<FName> fnameSerializer,
        IStreamSerializer<ObjectIndex> objectIndexSerializer)
    {
        _stateSerializer = stateSerializer;
        _fnameSerializer = fnameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UClass obj, IUnrealPackageStream objectStream)
    {
        _stateSerializer.DeserializeObject(obj, objectStream);
        obj.ClassFlags = objectStream.ReadUInt32();
        // RL specific
        objectStream.BaseStream.Move(4);

        obj.Within = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream));
        obj.ConfigName = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream.BaseStream));

        var componentCount = objectStream.ReadInt32();
        for (var i = 0; i < componentCount; i++)
        {
            var name = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream.BaseStream));
            if (obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream)) is UComponent component)
            {
                obj.ComponentNameToDefaultObjectMap.Add(name, component);
            }
        }

        var interfaceCount = objectStream.ReadInt32();
        for (var i = 0; i < interfaceCount; i++)
        {
            var clz = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream)) as UClass;
            var prop = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream)) as UProperty;
            if (clz != null && prop != null)
            {
                obj.InterfaceMap.Add(clz, prop);
            }
        }

        obj.DontSortCategories = _fnameSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.HideCategories = _fnameSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.AutoExpandCategories = _fnameSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.AutoCollapseCategories = _fnameSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.ForceScriptOrder = objectStream.ReadInt32();
        obj.ClassGroups = _fnameSerializer.ReadTArrayToList(objectStream.BaseStream);
        obj.NativeClassName = objectStream.ReadFString();
        // None FName always?
        var idk1 = objectStream.ReadInt32();
        var idk2 = objectStream.ReadInt32();
        var idk3 = objectStream.ReadInt32();

        obj.DllBindNameOrDummy = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream.BaseStream));
        obj.DefaultObject = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream));
        var stateCount = objectStream.ReadInt32();
        for (var i = 0; i < stateCount; i++)
        {
            var stateName = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream.BaseStream));
            if (obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream.BaseStream)) is UState stateObj)
            {
                obj.StateMap.Add(stateName, stateObj);
            }
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UClass obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}