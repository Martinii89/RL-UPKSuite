using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Core.Struct;

public class DefaultQuatSerializer : IStreamSerializer<FQuat>
{
    /// <inheritdoc />
    public FQuat Deserialize(Stream stream)
    {
        return new FQuat
        {
            X = stream.ReadSingle(),
            Y = stream.ReadSingle(),
            Z = stream.ReadSingle(),
            W = stream.ReadSingle()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FQuat value)
    {
        stream.Write(value.X);
        stream.Write(value.Y);
        stream.Write(value.Z);
        stream.Write(value.W);
    }
}