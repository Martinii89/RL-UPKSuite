using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types.FileSummeryInner;

namespace RlUpk.Core.Serialization.Default;

/// <inheritdoc />
public class FGenerationInfoSerializer : IStreamSerializer<FGenerationInfo>
{
    /// <inheritdoc />
    public FGenerationInfo Deserialize(Stream stream)
    {
        return new FGenerationInfo
        {
            ExportCount = stream.ReadInt32(),
            NameCount = stream.ReadInt32(),
            NetObjectCount = stream.ReadInt32()
        };
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FGenerationInfo value)
    {
        stream.Write(value.ExportCount);
        stream.Write(value.NameCount);
        stream.Write(value.NetObjectCount);
    }
}