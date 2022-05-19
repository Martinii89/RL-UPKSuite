using Core.Classes.Core;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object;

/// <summary>
///     Default implementation for a UObject serializer
/// </summary>
public class DefaultObjectSerializer : BaseObjectSerializer<UObject>
{
    /// <inheritdoc />
    public override void DeserializeObject(UObject obj, Stream objectStream)
    {
        obj.NetIndex = objectStream.ReadInt32();
    }

    /// <inheritdoc />
    public override void SerializeObject(UObject obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}