using FluentAssertions;

using RlUpk.Core.Classes;
using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Types;
using RlUpk.Core.Utility;
using RlUpk.TestUtils.TestUtilities;

using Xunit;

namespace RlUpk.Core.Test.Utility;

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