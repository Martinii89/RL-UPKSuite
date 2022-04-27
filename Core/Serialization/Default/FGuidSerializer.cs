using Core.Types;

namespace Core.Serialization.Default;

public class FGuidSerializer : IStreamSerializerFor<FGuid>
{
    public FGuid Deserialize(Stream stream)
    {
        return new FGuid
        {
            A = stream.ReadUInt32(),
            B = stream.ReadUInt32(),
            C = stream.ReadUInt32(),
            D = stream.ReadUInt32()
        };
    }

    public void Serialize(Stream stream, FGuid value)
    {
        throw new NotImplementedException();
    }
}