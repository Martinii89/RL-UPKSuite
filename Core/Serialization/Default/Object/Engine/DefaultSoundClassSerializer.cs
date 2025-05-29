using System.Diagnostics;

using RlUpk.Core.Classes.Core;
using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Engine;

/// <inheritdoc />
public class DefaultSoundClassSerializer : BaseObjectSerializer<USoundClass>
{
    private readonly IObjectSerializer<UObject> _objectSerializer;

    /// <inheritdoc />
    public DefaultSoundClassSerializer(IObjectSerializer<UObject> objectSerializer)
    {
        _objectSerializer = objectSerializer;
    }

    /// <inheritdoc />
    public override void DeserializeObject(USoundClass obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.DeserializeObject(obj, objectStream);

        var editorDataCount = objectStream.ReadInt32();
        if (editorDataCount != 0)
        {
            Debugger.Break();
        }

        //obj.EditorData = objectStream.ReadDictionary(stream => stream.ReadObject()!, stream => stream.ReadObject());
    }

    /// <inheritdoc />
    public override void SerializeObject(USoundClass obj, IUnrealPackageStream objectStream)
    {
        _objectSerializer.SerializeObject(obj, objectStream);

        //write editor data
        objectStream.WriteInt32(0);
    }
}