using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Serialization.Extensions;

namespace Core.Serialization.Default.Object;

public class DefaultTextBufferSerializer : BaseObjectSerializer<UTextBuffer>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultTextBufferSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(UTextBuffer obj, Stream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        obj.Pos = objectStream.ReadUInt32();
        obj.Top = objectStream.ReadUInt32();
        obj.ScriptText = objectStream.ReadFString();
    }

    /// <inheritdoc />
    public override void SerializeObject(UTextBuffer obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}