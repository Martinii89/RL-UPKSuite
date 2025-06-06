﻿using FluentAssertions;

using NSubstitute;

using RlUpk.Core.Flags;
using RlUpk.Core.RocketLeague;
using RlUpk.Core.RocketLeague.Decryption;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.RocketLeague;
using RlUpk.Core.Types;
using RlUpk.Core.Types.FileSummeryInner;
using RlUpk.TestUtils.TestUtilities;

using Syroot.BinaryData;

using Xunit;

namespace RlUpk.Core.Test.RocketLeague;

public class PackageUnpackerTests
{
    private readonly IStreamSerializer<FileSummary> _serializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary), RocketLeagueBase.FileVersion);

    [Fact]
    public void PackageUnpackerTest_UnpackKnownPackage_BinaryEqualToKnownOutput()
    {
        // Arrange
        var inputTest = File.OpenRead("TestData/RocketPass_Premium_T_SF.upk");
        var outputStream = new MemoryStream();
        var outputExpected = File.ReadAllBytes("TestData/RocketPass_Premium_T_SF_decrypted.upk");
        var decryptionProvider = new DecryptionProvider("keys.txt");
        
        // Act
        var packedFile = new RLPackageUnpacker(inputTest, decryptionProvider, _serializer);
        packedFile.Unpack(outputStream);
        var outputBuffer = outputStream.ToArray();

        // Assert 

        outputBuffer.Length.Should().Be(outputExpected.Length);
        packedFile.UnpackResult.Should().Be(UnpackResult.Success);
        outputBuffer.Should().Equal(outputExpected);
    }

    [Fact]
    public void PackageUnpackerTest_UnpackPackage_CompressionFlagUnset()
    {
        // Arrange
        var inputTest = File.OpenRead("TestData/RocketPass_Premium_T_SF.upk");
        var outputStream = new MemoryStream();
        var decryptionProvider = new DecryptionProvider("keys.txt");
        // Act

        var packedFile = new RLPackageUnpacker(inputTest, decryptionProvider, _serializer);
        packedFile.Unpack(outputStream);
        outputStream.Position = 0;
        var fileSummery = _serializer.Deserialize(outputStream);

        // Assert 
        fileSummery.CompressionFlags.Should().Be(ECompressionFlags.CompressNone);
    }

    [Fact]
    public void PackageUnpackerTest_UnpackPackageWithMissingKey_DeserializationStateShouldBeFail()
    {
        // Arrange
        var inputTest = File.OpenRead("TestData/RocketPass_Premium_T_SF.upk");
        var outputStream = new MemoryStream();
        var decryptionProvider = Substitute.For<IDecrypterProvider>();
        decryptionProvider.DecryptionKeys.Returns(new List<byte[]>());

        // Act
        var unpacked = new RLPackageUnpacker(inputTest, decryptionProvider, _serializer);

        // Assert 
        unpacked.UnpackResult.Should().NotBe(UnpackResult.Success);
        unpacked.Valid.Should().Be(false);
    }
}