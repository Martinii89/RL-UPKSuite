using System.IO;
using System.Linq;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using FluentAssertions;
using Xunit;

namespace Core.Types.Tests;

public class UnrealPackageTests
{
    [Fact]
    public void DeserializeTest()
    {
        // Arrange
        var inputTest = File.OpenRead("TestData/RocketPass_Premium_T_SF.upk");
        var outputStream = new MemoryStream();
        var decryptionProvider = new DecryptionProvider("keys.txt");

        var unpacked = new PackageUnpacker(inputTest, outputStream, decryptionProvider);
        var unrealPackage = new UnrealPackage();
        outputStream.Position = 0;
        // Act

        unrealPackage.Deserialize(outputStream);

        // Assert 
        var names = unrealPackage.NameTable.Names;
        names.Count.Should().Be(unpacked.FileSummary.NameCount);
        names.First().Name.Should().Be("ArrayProperty");

        var imports = unrealPackage.ImportTable.Imports;
        imports.Count.Should().Be(unpacked.FileSummary.ImportCount);
        imports.First().ClassPackage.NameIndex.Should().Be(2);
        imports[5].ObjectName.NameIndex.Should().Be(12);

        var exports = unrealPackage.ExportTable.Exports;
        exports.Count.Should().Be(unpacked.FileSummary.ExportCount);
        exports[0].SerialSize.Should().Be(44);
        exports[1].SerialSize.Should().Be(12);
    }
}