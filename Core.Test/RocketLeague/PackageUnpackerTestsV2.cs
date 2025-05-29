using FluentAssertions;

using NSubstitute;

using RlUpk.Core.RocketLeague;
using RlUpk.Core.RocketLeague.Decryption;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.RocketLeague;
using RlUpk.Core.Types;
using RlUpk.Core.Types.FileSummeryInner;
using RlUpk.TestUtils.TestUtilities;

using Xunit;

namespace RlUpk.Core.Test.RocketLeague;

public class PackageUnpackerTestsV2
{
    private readonly IStreamSerializer<FileSummary> _serializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary), RocketLeagueBase.FileVersion);
    private readonly DecryptionProvider _decryptionProvider = new ("keys.txt");
    private readonly RLPackageUnpackerV2 _sut;

    public PackageUnpackerTestsV2()
    {
        _sut = new RLPackageUnpackerV2(_decryptionProvider, _serializer);
    }
    
    
    [Fact]
    public void PackageUnpackerTest_UnpackKnownPackage_BinaryEqualToKnownOutput()
    {
        // Arrange
        var inputTest = File.OpenRead("TestData/RocketPass_Premium_T_SF.upk");
        var outputStream = new MemoryStream();
        var outputExpected = File.ReadAllBytes("TestData/RocketPass_Premium_T_SF_decrypted.upk");
        
        // Act
        var result = _sut.Unpack(inputTest, outputStream);
        var outputBuffer = outputStream.ToArray();

        // Assert 
        outputBuffer.Length.Should().Be(outputExpected.Length);
        result.Should().Be(UnpackResult.Success);
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

        var result = _sut.Unpack(inputTest, outputStream);
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
        var unpackerV2 = new RLPackageUnpackerV2(decryptionProvider, _serializer);

        // Act
        var result = unpackerV2.Unpack(inputTest, outputStream);

        // Assert 
        result.HasFlag(UnpackResult.Decrypted).Should().BeFalse();
    }
}