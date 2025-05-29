using System.Diagnostics;

using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

public class DefaultDecalComponentSerializer : BaseObjectSerializer<UDecalComponent>
{
    private readonly IObjectSerializer<UComponent> _componentSerializer;

    public DefaultDecalComponentSerializer(IObjectSerializer<UComponent> componentSerializer)
    {
        _componentSerializer = componentSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UDecalComponent obj, IUnrealPackageStream objectStream)
    {
        _componentSerializer.DeserializeObject(obj, objectStream);

        obj.NumStaticReceivers = objectStream.ReadInt32();
        if (obj.NumStaticReceivers > 0)
        {
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(UDecalComponent obj, IUnrealPackageStream objectStream)
    {
        _componentSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteInt32(obj.NumStaticReceivers);
    }
}