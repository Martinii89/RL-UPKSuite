using RlUpk.Core.Classes.Core;
using RlUpk.Core.Flags;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object;

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
            obj.StateFrame.Node = objectStream.ReadObject();
            obj.StateFrame.StateNode = objectStream.ReadObject();
            obj.StateFrame.ProbeMask = objectStream.ReadUInt32();
            obj.StateFrame.LatentAction = objectStream.ReadUInt16();
            obj.StateFrame.StateStackCount = objectStream.ReadUInt32();
            obj.StateFrame.Offset = objectStream.ReadInt32();
        }

        obj.NetIndex = objectStream.ReadInt32();

        if (obj.Class == obj.OwnerPackage.StaticClass)
        {
            return;
        }

        obj.ScriptProperties.AddRange(_scriptPropertiesSerializer.GetScriptProperties(obj, objectStream));
    }

    /// <inheritdoc />
    public override void SerializeObject(UObject obj, IUnrealPackageStream objectStream)
    {
        if (obj.HasObjectFlag(ObjectFlagsLO.HasStack))
        {
            objectStream.WriteObject(obj.StateFrame.Node);
            objectStream.WriteObject(obj.StateFrame.StateNode);
            objectStream.WriteUInt32(obj.StateFrame.ProbeMask);
            objectStream.WriteUInt16(obj.StateFrame.LatentAction);
            objectStream.WriteUInt32(obj.StateFrame.StateStackCount);
            objectStream.WriteInt32(obj.StateFrame.Offset);
        }
        
        objectStream.WriteInt32(obj.NetIndex); // seeing a bunch of warnings about bad net indexes are neat for debugging purposes

        if (obj.Class == obj.OwnerPackage.StaticClass)
        {
            return;
        }

        _scriptPropertiesSerializer.WriteScriptProperties(obj.ScriptProperties, obj, objectStream);
    }
}