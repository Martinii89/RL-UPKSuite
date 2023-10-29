using Core.Classes.Core.Structs;
using Core.Classes.Engine.Structs;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class DefaultColorStreamSerializer : IStreamSerializer<ColorStream>
{
    private readonly IStreamSerializer<FColor> _colorSerializer;

    public DefaultColorStreamSerializer(IStreamSerializer<FColor> colorSerializer)
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
        stream.WriteInt32(value.ItemSize);
        stream.WriteInt32(value.NumVerts);
        if (value.NumVerts > 0)
        {
            _colorSerializer.BulkWriteTArray(stream, value.Colors);
        }
    }
}