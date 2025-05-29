using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Core.Struct;

public class DefaultVectorSerializer : IStreamSerializer<FVector>
{
    /// <inheritdoc />
    public FVector Deserialize(Stream stream)
    {
        return new FVector
        {
            X = stream.ReadSingle(),
            Y = stream.ReadSingle(),
            Z = stream.ReadSingle()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FVector value)
    {
        stream.Write(value.X);
        stream.Write(value.Y);
        stream.Write(value.Z);
    }
}