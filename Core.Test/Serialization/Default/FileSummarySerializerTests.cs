using System.IO;
using Core.Types;
using Core.Types.FileSummeryInner;
using FluentAssertions;
using Xunit;

namespace Core.Serialization.Default.Tests;

public class FileSummarySerializerTests
{
    [Fact]
    public void FileSummarySerializerTest_CanDeserializeCorrectData()
    {
        // Arrange
        // File    : D:\Projects\RL UPKSuite\Core.Test\TestData\RocketPass_Premium_T_SF.upk
        // Address : 0 (0x0)
        // Size    : 153 (0x99)
        var headerData = new byte[]
        {
            0xC1, 0x83, 0x2A, 0x9E, 0x64, 0x03, 0x20, 0x00, 0xAD, 0x43, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00,
            0x4E, 0x6F, 0x6E, 0x65, 0x00, 0x8D, 0x02, 0x88, 0x02, 0x0D, 0x00, 0x00, 0x00, 0x99, 0x00, 0x00,
            0x00, 0x04, 0x00, 0x00, 0x00, 0x97, 0x02, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0xEF, 0x01, 0x00,
            0x00, 0xB7, 0x03, 0x00, 0x00, 0xB7, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0xD1, 0x2B, 0xFF, 0x91, 0xDF, 0xA6, 0x73, 0x41, 0xB8, 0xDD, 0xB7,
            0xC8, 0x0D, 0x7C, 0xEE, 0x5A, 0x02, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x91, 0x2A, 0x00, 0x00, 0x88, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0xF0, 0x88, 0x8C, 0x25, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xDA, 0x3F, 0x00,
            0x00, 0x1E, 0x03, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00
        };
        var reader = new MemoryStream(headerData);

        var mainSerializer = new FileSummarySerializer(new FGuidSerializer(), new FCompressedChunkSerializer(),
            new FStringSerializer(), new FTextureAllocationsSerializer(),
            new FGenerationInfoSerializer());

        // Act
        var sut = mainSerializer.Deserialize(reader);

        // Assert 
        sut.Tag.Should().Be(FileSummary.PackageFileTag);
        sut.FileVersion.Should().Be(868);
        sut.LicenseeVersion.Should().Be(32);
        sut.TotalHeaderSize.Should().Be(17325);
        sut.FolderName.Should().Be("None");
        sut.PackageFlags.Should().Be(0x288028D);
        sut.NameCount.Should().Be(13);
        sut.NameOffset.Should().Be(153);
        sut.ExportCount.Should().Be(4);
        sut.ExportOffset.Should().Be(663);
        sut.ImportCount.Should().Be(6);
        sut.ImportOffset.Should().Be(495);
        sut.DependsOffset.Should().Be(951);
        sut.ImportExportGuidsOffset.Should().Be(951);
        sut.ImportGuidsCount.Should().Be(0);
        sut.ExportGuidsCount.Should().Be(0);
        sut.ThumbnailTableOffset.Should().Be(0);
        sut.Guid.A.Should().Be(2449419217);
        sut.Guid.B.Should().Be(1098098399);
        sut.Guid.C.Should().Be(3367493048);
        sut.Guid.D.Should().Be(1525578765);
        sut.Generations.Count.Should().Be(2);
        sut.Generations[0].ExportCount.Should().Be(4);
        sut.EngineVersion.Should().Be(10897);
        sut.CookerVersion.Should().Be(136);
        sut.CompressionFlags.Should().Be(ECompressionFlags.CompressZlib);
        sut.AdditionalPackagesToCook.Count.Should().Be(0);
        sut.TextureAllocations.Count.Should().Be(0);
    }
}