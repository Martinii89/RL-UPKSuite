using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Properties;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class RLClassSerializer : BaseObjectSerializer<UClass>
{
    private readonly IObjectSerializer<UState> _stateSerializer;

    public RLClassSerializer(IObjectSerializer<UState> stateSerializer)
    {
        _stateSerializer = stateSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UClass obj, IUnrealPackageStream objectStream)
    {
        _stateSerializer.DeserializeObject(obj, objectStream);
        obj.ClassFlags = objectStream.ReadUInt32();
        // RL specific
        objectStream.BaseStream.Move(4);

        obj.Within = objectStream.ReadObject();
        obj.ConfigName = objectStream.ReadFNameStr();

        var componentCount = objectStream.ReadInt32();
        for (var i = 0; i < componentCount; i++)
        {
            var name = objectStream.ReadFNameStr();
            if (objectStream.ReadObject() is UComponent component)
            {
                obj.ComponentNameToDefaultObjectMap.Add(name, component);
            }
        }

        var interfaceCount = objectStream.ReadInt32();
        for (var i = 0; i < interfaceCount; i++)
        {
            var clz = objectStream.ReadObject() as UClass;
            var prop = objectStream.ReadObject() as UProperty;
            if (clz != null && prop != null)
            {
                obj.InterfaceMap.Add(clz, prop);
            }
        }

        obj.DontSortCategories = objectStream.ReadTArray(stream => stream.ReadFName());
        obj.HideCategories = objectStream.ReadTArray(stream => stream.ReadFName());
        obj.AutoExpandCategories = objectStream.ReadTArray(stream => stream.ReadFName());
        obj.AutoCollapseCategories = objectStream.ReadTArray(stream => stream.ReadFName());
        obj.ForceScriptOrder = objectStream.ReadInt32();
        obj.ClassGroups = objectStream.ReadTArray(stream => stream.ReadFName());
        obj.NativeClassName = objectStream.ReadFString();
        // None FName always?
        var idk1 = objectStream.ReadInt32();
        var idk2 = objectStream.ReadInt32();
        var idk3 = objectStream.ReadInt32();

        obj.DllBindNameOrDummy = objectStream.ReadFNameStr();
        obj.DefaultObject = objectStream.ReadObject();
        var stateCount = objectStream.ReadInt32();
        for (var i = 0; i < stateCount; i++)
        {
            var stateName = objectStream.ReadFNameStr();
            if (objectStream.ReadObject() is UState stateObj)
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