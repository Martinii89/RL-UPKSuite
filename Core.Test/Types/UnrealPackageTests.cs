using Xunit;
using System;
using System.IO;
using System.Linq;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using FluentAssertions;

namespace Core.Types.Tests
{
    public class UnrealPackageTests
    {
        [Fact()]
        public void DeserializeTest()
        {
            // Arrange
            var inputTest = File.OpenRead("TestData/RocketPass_Premium_T_SF.upk");
            var outputStream = new MemoryStream();
            var decryptionProvider = new DecryptionProvider("keys.txt");

            var unpacked = new PackageUnpacker(inputTest, outputStream, decryptionProvider);
            var unrealPackage = new UnrealPackage();
            outputStream.Position = 0;
            var binaryStream = new BinaryReader(outputStream);
            // Act

            unrealPackage.Deserialize(binaryStream);

            // Assert 
            var names = unrealPackage.NameTable.Names;
            names.Count.Should().Be(unpacked.FileSummary.NameCount);
            names.First().Name.Should().Be("ArrayProperty");

            throw new NotImplementedException("not done");

        }
    }
}
