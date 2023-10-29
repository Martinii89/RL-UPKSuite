using System.Diagnostics;
using Core.Classes.Core;
using Core.Classes.Engine;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object.Engine;

public class DefaultSoundCueSerializer : BaseObjectSerializer<USoundCue>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    public DefaultSoundCueSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(USoundCue obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        var editorDataCount = objectStream.ReadInt32();
        if (editorDataCount > 0)
        {
            // should never happen when loading
            Debugger.Break();
        }
    }

    /// <inheritdoc />
    public override void SerializeObject(USoundCue obj, IUnrealPackageStream objectStream)
    {
        throw new NotImplementedException();
    }
}