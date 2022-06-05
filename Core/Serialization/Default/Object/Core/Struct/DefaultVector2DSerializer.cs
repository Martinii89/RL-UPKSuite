using Core.Classes.Core.Structs;

namespace Core.Serialization.Default.Object.Core.Struct;

public class DefaultVector2DSerializer : IStreamSerializerFor<FVector2D>
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