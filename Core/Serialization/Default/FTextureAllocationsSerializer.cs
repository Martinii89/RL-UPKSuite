using Core.Serialization.Extensions;
using Core.Types.FileSummeryInner;

namespace Core.Serialization.Default;

/// <inheritdoc />
public class FTextureAllocationsSerializer : IStreamSerializer<FTextureType>
{
    /// <inheritdoc />
    public FTextureType Deserialize(Stream stream)
    {
        var type = new FTextureType();
        type.SizeX = stream.ReadInt32();
        type.SizeY = stream.ReadInt32();
        type.NumMips = stream.ReadInt32();
        type.Format = stream.ReadInt32();
        type.TexCreateFlags = stream.ReadInt32();
        type.ExportIndices = stream.ReadTarray(stream1 => stream1.ReadInt32());
        return type;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FTextureType value)
    {
        stream.Write(value.SizeX);
        stream.Write(value.SizeY);
        stream.Write(value.NumMips);
        stream.Write(value.Format);
        stream.Write(value.TexCreateFlags);
        stream.WriteTArray(value.ExportIndices, (stream1, i) => stream1.WriteInt32(i));
    }
}