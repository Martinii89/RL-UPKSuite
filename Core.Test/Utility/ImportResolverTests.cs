using Core.Serialization;
using Core.Test.TestUtilities;
using Core.Types;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Core.Utility.Tests;

public class ImportResolverTests : SerializerTestBase
{
    private readonly IStreamSerializerFor<UnrealPackage> _serializer;

    public ImportResolverTests()
    {
        _serializer = GetSerializersCollection(typeof(UnrealPackage)).GetRequiredService<IStreamSerializerFor<UnrealPackage>>();
    }

    [Fact]
    public void ImportResolverTest_CanFindCoreInTestData()
    {
        // Arrange
        var options = new ImportResolverOptions(_serializer) { SearchPaths = { @"TestData/UDK/" } };
        var resolver = new ImportResolver(options);
        // Act

        var resolvedPackage = resolver.ResolveExportPackage("Core");
        // Assert 
        resolvedPackage.Should().NotBeNull();
    }
}