using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Core.Properties;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object;

public class DefaultClassSerializer : BaseObjectSerializer<UClass>
{
    private readonly IObjectSerializer<UState> _stateSerializer;

    public DefaultClassSerializer(IObjectSerializer<UState> stateSerializer)
    {
        _stateSerializer = stateSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UClass obj, IUnrealPackageStream objectStream)
    {
        _stateSerializer.DeserializeObject(obj, objectStream);
        obj.ClassFlags = objectStream.ReadUInt32();
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

        obj.DontSortCategories = objectStream.ReadTArray(stream => objectStream.ReadFName());
        obj.HideCategories = objectStream.ReadTArray(stream => objectStream.ReadFName());
        obj.AutoExpandCategories = objectStream.ReadTArray(stream => objectStream.ReadFName());
        obj.AutoCollapseCategories = objectStream.ReadTArray(stream => objectStream.ReadFName());
        obj.ForceScriptOrder = objectStream.ReadInt32();
        obj.ClassGroups = objectStream.ReadTArray(stream => objectStream.ReadFName());
        obj.NativeClassName = objectStream.ReadFString();
        // None FName always?
        obj.DllBindNameOrDummy = objectStream.ReadFNameStr();
        obj.DefaultObject = objectStream.ReadObject();
    }

    /// <inheritdoc />
    public override void SerializeObject(UClass obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}