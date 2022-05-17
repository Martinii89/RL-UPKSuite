using Core.Classes.Core;
using Core.Serialization.Abstraction;

namespace Core.Serialization.Default.Object;

public class DefaultObjectSerializer : IObjectSerializer<UObject>
{
    public void DeserializeObject(UObject obj, Stream objectStream)
    {
        obj.NetIndex = objectStream.ReadInt32();
    }

    public void SerializeObject(UObject obj, Stream objectStream)
    {
        throw new NotImplementedException();
    }
}