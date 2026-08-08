using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types.FileSummeryInner;

namespace RlUpk.Core.Serialization.RocketLeague;

/// <inheritdoc />
[FileVersion(RocketLeagueBase.FileVersion)]
public class FCompressedChunkSerializer : IStreamSerializer<FCompressedChunk>, ILicenseeVersionAwareSerializer
{
    private const ushort PaddingLicenseeVersion = 33;
    private const int PaddingSize = 12;

    /// <summary>
    ///     The licensee version controls the format of the compressed chunk.
    /// </summary>
    public ushort LicenseeVersion { get; set; }

    /// <summary>
    ///     Constructs a Rocket League compressed chunk serializer.
    /// </summary>
    public FCompressedChunkSerializer()
    {
    }

    /// <summary>
    ///     Constructs a Rocket League compressed chunk serializer for a package version.
    /// </summary>
    /// <param name="licenseeVersion">The licensee version of the package.</param>
    public FCompressedChunkSerializer(ushort licenseeVersion)
    {
        LicenseeVersion = licenseeVersion;
    }

    /// <inheritdoc />
    public FCompressedChunk Deserialize(Stream stream)
    {
        var value = new FCompressedChunk
        {
            UncompressedOffset = stream.ReadInt64(),
            UncompressedSize = stream.ReadInt32(),
            CompressedOffset = stream.ReadInt64(),
            CompressedSize = stream.ReadInt32()
        };

        if (LicenseeVersion >= PaddingLicenseeVersion)
        {
            stream.Move(PaddingSize);
        }

        return value;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, FCompressedChunk value)
    {
        stream.WriteInt64(value.UncompressedOffset);
        stream.WriteInt32(value.UncompressedSize);
        stream.WriteInt64(value.CompressedOffset);
        stream.WriteInt32(value.CompressedSize);

        if (LicenseeVersion >= PaddingLicenseeVersion)
        {
            stream.Write(new byte[PaddingSize]);
        }
    }
}
