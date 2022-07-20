using System.IO;
using Core.Test.TestUtilities;
using Core.Types;
using FluentAssertions;
using Xunit;

namespace Core.Utility.Tests;

public class PackageExporterTests
{
    private readonly UnrealPackage _testPackage;

    public PackageExporterTests()
    {
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        var nativeFactory = new NativeClassFactory();
        var options = new PackageCacheOptions(serializer, nativeFactory) { SearchPaths = { @"TestData/UDK/" }, GraphLinkPackages = false };
        var packageCache = new PackageCache(options);
        var loader = new PackageLoader(serializer, packageCache, new NeverUnpackUnpacker(), nativeFactory);
        loader.LoadPackage("TestData/UDK/UDKTestPackage.upk", "UDKTestPackage");
        _testPackage = loader.GetPackage("UDKTestPackage");
    }

    [Fact]
    public void PackageExporterTest()
    {
        Assert.True(false, "This test needs an implementation");
    }

    [Fact]
    public void ExportHeader_StreamShouldBeSerializableToEqualStructl()
    {
        // Arrange
        var serializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary));
        var stream = new MemoryStream();
        var sut = new PackageExporter(_testPackage, stream, serializer);

        // Act

        sut.ExportHeader();
        stream.Position = 0;
        var serializedHeader = serializer.Deserialize(stream);
        // Assert

        serializedHeader.Should().BeEquivalentTo(_testPackage.Header);
    }
}