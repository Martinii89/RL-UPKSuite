using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultColorStreamSerializer : IStreamSerializerFor<ColorStream>
{
    private readonly IStreamSerializerFor<FColor> _colorSerializer;

    public DefaultColorStreamSerializer(IStreamSerializerFor<FColor> colorSerializer)
    {
        _colorSerializer = colorSerializer;
    }

    /// <inheritdoc />
    public ColorStream Deserialize(Stream stream)
    {
        var colorStream = new ColorStream
        {
            ItemSize = stream.ReadInt32(),
            NumVerts = stream.ReadInt32()
        };
        if (colorStream.NumVerts > 0)
        {
            colorStream.Colors = _colorSerializer.ReadTArrayWithElementSize(stream);
        }

        return colorStream;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, ColorStream value)
    {
        throw new NotImplementedException();
    }
}