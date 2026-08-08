using FluentAssertions;

using RlUpk.Core.Serialization.RocketLeague;
using RlUpk.Core.Types.FileSummeryInner;

using Xunit;

namespace RlUpk.Core.Test.Serialization.RocketLeague;

public class FCompressedChunkSerializerTests
{
    [Theory]
    [InlineData((ushort) 32, 24)]
    [InlineData((ushort) 33, 36)]
    public void Serialize_UsesLicenseeVersionSpecificSize(ushort licenseeVersion, int expectedSize)
    {
        var serializer = new FCompressedChunkSerializer(licenseeVersion);
        using var stream = new MemoryStream();

        serializer.Serialize(stream, new FCompressedChunk());

        stream.Length.Should().Be(expectedSize);
    }

    [Fact]
    public void Deserialize_LicenseeVersion33_SkipsPadding()
    {
        var serializer = new FCompressedChunkSerializer(33);
        using var stream = new MemoryStream();
        serializer.Serialize(stream, new FCompressedChunk());
        stream.WriteByte(0x7F);
        stream.Position = 0;

        serializer.Deserialize(stream);

        stream.Position.Should().Be(36);
        stream.ReadByte().Should().Be(0x7F);
    }
}
