using FluentAssertions;

using RlUpk.Core.Classes.Compression;
using RlUpk.Core.Serialization.Default;

using Xunit;

namespace RlUpk.Core.Test.Classes.Compression;

public class PackageCompressorTests
{
    [Fact]
    public void CompressFile_ShouldNotThrow_WhenCompressingOctanePackage()
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
        
        outputPackage.Dispose();
        var result = File.ReadAllBytes("TestData/Body_Octane_SF_exported_zlib.upk");
        var target = File.ReadAllBytes("TestData/Body_Octane_SF_exported_zlib_valid.upk");

        result.Should().BeEquivalentTo(target);
    }
}