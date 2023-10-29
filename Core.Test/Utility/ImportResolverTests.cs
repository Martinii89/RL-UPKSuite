using Core.Serialization;
using Core.Test.TestUtilities;
using Core.Types;
using FluentAssertions;
using Xunit;

namespace Core.Utility.Tests;

public class ImportResolverTests : SerializerHelper
{
    private readonly IStreamSerializer<UnrealPackage> _serializer;

    public ImportResolverTests()
    {
        _serializer = GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
    }

    [Fact]
    public void ImportResolverTest_CanFindCoreInTestData()
    {
        // Arrange
        var nativeFactory = new NativeClassFactory();
        var options = new PackageCacheOptions(_serializer, nativeFactory) { SearchPaths = { @"TestData/UDK/" } };
        var resolver = new PackageCache(options);
        // Act

        var resolvedPackage = resolver.ResolveExportPackage("Core");
        // Assert 
        resolvedPackage.Should().NotBeNull();
    }
}