using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.Default.Object.Core.Struct;

public class DefaultVector2DSerializer : IStreamSerializer<FVector2D>
{
    /// <inheritdoc />
    public FVector2D Deserialize(Stream stream)
    {
        return new FVector2D
        {
            X = stream.ReadSingle(),
            Y = stream.ReadSingle()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FVector2D value)
    {
        stream.Write(value.X);
        stream.Write(value.Y);
    }
}