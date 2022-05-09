using Core.Test.TestUtilities;
using Core.Types;
using Core.Utility;
using FluentAssertions;
using Xunit;

namespace Core.Tests;

public class PackageLoaderTests
{
    [Theory]
    [InlineData("CustomGame")]
    [InlineData("Engine")]
    [InlineData("GameFramework")]
    [InlineData("ProjectX")]
    [InlineData("AKAudio")]
    [InlineData("TAGame")]
    public void LoadPackageTest_FullyLoad_AllObjectsShouldBeCreated(string packageName)
    {
        // Arrange
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        var options = new ImportResolverOptions(serializer) { SearchPaths = { @"TestData/UDK/" }, GraphLinkPackages = false };
        var packageCache = new PackageCache(options);
        var loader = new PackageLoader(serializer, packageCache);

        // Act
        loader.LoadPackage($"TestData/UDK/{packageName}.u", packageName);
        var pckg = loader.GetPackage(packageName);
        // Assert 
        pckg.ExportTable.Should().AllSatisfy(x => { x.Object.Should().NotBeNull(); });
        pckg.ImportTable.Should().AllSatisfy(x => { x.ImportedObject.Should().NotBeNull(); });
    }
}