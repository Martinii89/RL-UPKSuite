using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Core.Struct;

public class DefaultMatrixSerializer : IStreamSerializer<FMatrix>
{
    /// <inheritdoc />
    public FMatrix Deserialize(Stream stream)
    {
        return new FMatrix
        {
            M = stream.ReadSingles(16)
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FMatrix value)
    {
        stream.WriteSingles(value.M);
    }
}