using System.Linq;
using Core.Classes;
using Core.Classes.Core;
using Core.Classes.Core.Properties;
using Core.Serialization;
using Core.Test.TestUtilities;
using FluentAssertions;
using Xunit;

namespace Core.Types.Tests;

public class ObjectFactoryTests : IClassFixture<PackageStreamFixture>
{
    private readonly PackageStreamFixture _packageStreams;
    private readonly IStreamSerializerFor<UnrealPackage> _udkPackageSerializer;

    public ObjectFactoryTests(PackageStreamFixture packageStreams)
    {
        _packageStreams = packageStreams;
        _udkPackageSerializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
    }

    [Fact]
    public void CorePackageLink_ExportedClassesShouldBeUClassObjects()
    {
        // Arrange
        var package = UnrealPackage.DeserializeAndInitialize(_packageStreams.CoreStream, new UnrealPackageOptions(_udkPackageSerializer, "Core", null));

        // Act
        package.GraphLink();
        var sut = package.ExportTable.Where(x => x?.Object?.Class?.Name == "Class").Select(x => x.Object);

        // Assert 
        sut.Should().NotBeEmpty();
        sut.Should().AllBeOfType<UClass>();
    }

    [Fact]
    public void CorePackageLink_ExportedEnumsShouldBeUEnumObjects()
    {
        // Arrange
        var package = UnrealPackage.DeserializeAndInitialize(_packageStreams.CoreStream, new UnrealPackageOptions(_udkPackageSerializer, "Core", null));

        // Act
        package.GraphLink();
        var sut = package.ExportTable.Where(x => x?.Object?.Class?.Name == "Enum").Select(x => x.Object);

        // Assert 
        sut.Should().NotBeEmpty();
        sut.Should().AllBeOfType<UEnum>();
    }


    [Fact]
    public void CorePackageLink_ExportedFunctionsShouldBeUFunctionObjects()
    {
        // Arrange
        var package = UnrealPackage.DeserializeAndInitialize(_packageStreams.CoreStream, new UnrealPackageOptions(_udkPackageSerializer, "Core", null));

        // Act
        package.GraphLink();
        var sut = package.ExportTable.Where(x => x?.Object?.Class?.Name == "Function").Select(x => x.Object);

        // Assert 
        sut.Should().NotBeEmpty();
        sut.Should().AllBeOfType<UFunction>();
    }

    [Fact]
    public void CorePackageLink_ExportedFunctionsShouldBeUFloatPropertyObjects()
    {
        // Arrange
        var package = UnrealPackage.DeserializeAndInitialize(_packageStreams.CoreStream, new UnrealPackageOptions(_udkPackageSerializer, "Core", null));

        // Act
        package.GraphLink();
        var sut = package.ExportTable.Where(x => x?.Object?.Class?.Name == "FloatProperty").Select(x => x.Object);

        // Assert 
        sut.Should().NotBeEmpty();
        sut.Should().AllBeOfType<UFloatProperty>();
    }
}