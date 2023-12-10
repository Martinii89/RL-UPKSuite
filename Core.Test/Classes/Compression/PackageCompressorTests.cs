using Core.Serialization.Default;
using FluentAssertions;
using Xunit;

namespace Core.Classes.Compression.Tests;

public class PackageCompressorTests
{
    [Fact]
    public void CompressFileTest()
    {
        // Arrange
        var headerSerializer = FileSummarySerializer.GetDefaultSerializer();
        var exportTableIteSerializer =
            new ExportTableItemSerializer(new FNameSerializer(), new ObjectIndexSerializer(), new FGuidSerializer());
        using var testPackage = File.OpenRead("TestData/Body_Octane_SF_exported.upk");
        using var outputPackage = File.OpenWrite("TestData/Body_Octane_SF_exported_zlib.upk");
        var sut = new PackageCompressor(headerSerializer, exportTableIteSerializer, new FCompressedChunkinfoSerializer());
        // Act

        var act = () => sut.CompressFile(testPackage, outputPackage);

        // Assert 
        act.Should().NotThrow();
    }
}