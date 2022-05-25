using System.Diagnostics;
using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;
using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class RLClassSerializer : BaseObjectSerializer<UClass>
{
    private readonly IStreamSerializerFor<FName> _fnameSerializer;
    private readonly IStreamSerializerFor<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializer<UState> _stateSerializer;

    public RLClassSerializer(IObjectSerializer<UState> stateSerializer, IStreamSerializerFor<FName> fnameSerializer,
        IStreamSerializerFor<ObjectIndex> objectIndexSerializer)
    {
        _stateSerializer = stateSerializer;
        _fnameSerializer = fnameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UClass obj, Stream objectStream)
    {
        _stateSerializer.DeserializeObject(obj, objectStream);
        obj.ClassFlags = objectStream.ReadUInt32();
        // RL specific
        objectStream.Move(4);

        obj.Within = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        obj.ConfigName = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));

        var componentCount = objectStream.ReadInt32();
        for (var i = 0; i < componentCount; i++)
        {
            var name = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
            if (obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) is UComponent component)
            {
                obj.ComponentNameToDefaultObjectMap.Add(name, component);
            }
        }

        var interfaceCount = objectStream.ReadInt32();
        for (var i = 0; i < interfaceCount; i++)
        {
            var clz = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UClass;
            var prop = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream)) as UProperty;
            if (clz != null && prop != null)
            {
                obj.InterfaceMap.Add(clz, prop);
            }
        }

        obj.DontSortCategories = _fnameSerializer.ReadTArrayToList(objectStream);
        obj.HideCategories = _fnameSerializer.ReadTArrayToList(objectStream);
        obj.AutoExpandCategories = _fnameSerializer.ReadTArrayToList(objectStream);
        obj.AutoCollapseCategories = _fnameSerializer.ReadTArrayToList(objectStream);
        obj.ForceScriptOrder = objectStream.ReadInt32();
        obj.ClassGroups = _fnameSerializer.ReadTArrayToList(objectStream);
        obj.NativeClassName = objectStream.ReadFString();
        // None FName always?
        var idk1 = objectStream.ReadInt32();
        var idk2 = objectStream.ReadInt32();
        var idk3 = objectStream.ReadInt32();

        obj.DllBindNameOrDummy = obj.OwnerPackage.GetName(_fnameSerializer.Deserialize(objectStream));
        obj.DefaultObject = obj.OwnerPackage.GetObject(_objectIndexSerializer.Deserialize(objectStream));
        objectStream.Move(4);
        if (objectStream.Position != obj.ExportTableItem!.SerialOffset + obj.ExportTableItem.SerialSize)
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UClass obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}