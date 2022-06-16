using Core.Classes.Core;
using Core.Flags;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object;

/// <summary>
///     Default implementation for a UObject serializer
/// </summary>
public class DefaultObjectSerializer : BaseObjectSerializer<UObject>
{
    private readonly ScriptPropertiesSerializer _scriptPropertiesSerializer;

    public DefaultObjectSerializer()
    {
        _scriptPropertiesSerializer = new ScriptPropertiesSerializer();
    }

    /// <inheritdoc />
    public override void DeserializeObject(UObject obj, IUnrealPackageStream objectStream)
    {
        if (obj.HasObjectFlag(ObjectFlagsLO.HasStack))
        {
            var node = objectStream.ReadObject();
            var stateNode = objectStream.ReadObject();
            var probeMask = objectStream.ReadUInt32();
            var latentAction = objectStream.ReadUInt16();
            var stateStackCount = objectStream.ReadUInt32();
            var offset = objectStream.ReadInt32();
        }

        obj.NetIndex = objectStream.ReadInt32();

        if (obj.Class == UClass.StaticClass)
        {
            return;
        }

        //obj.ScriptProperties.AddRange(GetScriptProperties(obj, objectStream));
        obj.ScriptProperties.AddRange(_scriptPropertiesSerializer.GetScriptProperties(obj, objectStream));
    }

    /// <inheritdoc />
    public override void SerializeObject(UObject obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}