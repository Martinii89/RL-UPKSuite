using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

/// <inheritdoc />
public class DefaultFontSerializer : BaseObjectSerializer<UFont>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    /// <inheritdoc />
    public DefaultFontSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UFont obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        obj.CharRemap = objectStream.ReadDictionary(stream => stream.ReadInt16(), stream => stream.ReadInt16());
    }

    /// <inheritdoc />
    public override void SerializeObject(UFont obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);
        objectStream.WriteDictionary(obj.CharRemap, (stream, val) => stream.WriteInt16(val), (stream, val) => stream.WriteInt16(val));
    }
}