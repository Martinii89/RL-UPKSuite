using Core.Classes.Core.Structs;

namespace Core.Serialization.Default.Object.Core.Struct;

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