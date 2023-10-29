using System.IO;
using System.Linq;
using Core.Serialization.RocketLeague;
using Core.Types;
using Core.Utility;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Core.Serialization.Default.Tests;

public class UnrealPackageSerializerTests
{
    [Fact]
    public void DeserializeTest_CanGetSerializerFromDI()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        // Act
        serviceColection.UseSerializers(typeof(UnrealPackageSerializer), new SerializerOptions());
        var services = serviceColection.BuildServiceProvider();
        var testSerializer = services.GetRequiredService<IStreamSerializer<UnrealPackage>>();
        // Assert
        testSerializer.Should().NotBeNull();
    }

    [Fact]
    public void SerializeTest_SerializeRocketLeagueTestFile_TablesCorrectLength()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        var inputTest = File.OpenRead(@"TestData/RocketPass_Premium_T_SF_decrypted.upk");
        serviceColection.UseSerializers(typeof(UnrealPackageSerializer), new SerializerOptions(RocketLeagueBase.FileVersion));
        var services = serviceColection.BuildServiceProvider();
        var testSerializer = services.GetRequiredService<IStreamSerializer<UnrealPackage>>();
        // Act

        var unrealPackage = testSerializer.Deserialize(inputTest);

        // Assert
        var names = unrealPackage.NameTable;
        names.Should().HaveCount(13);
        names.First().Name.Should().Be("ArrayProperty");

        var imports = unrealPackage.ImportTable;
        imports.Should().HaveCount(6);
        imports.First().ClassPackage.NameIndex.Should().Be(2);
        imports[5].ObjectName.NameIndex.Should().Be(12);

        var exports = unrealPackage.ExportTable;
        exports.Should().HaveCount(4);
        exports[0].SerialSize.Should().Be(44);
        exports[1].SerialSize.Should().Be(12);
    }

    [Fact]
    public void SerializeTest_SerializeUDKTestFile_TablesCorrectLength()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        var inputTest = File.OpenRead(@"TestData/UDK/UDKTestPackage.upk");
        serviceColection.UseSerializers(typeof(UnrealPackageSerializer), new SerializerOptions());
        var services = serviceColection.BuildServiceProvider();
        var testSerializer = services.GetRequiredService<IStreamSerializer<UnrealPackage>>();
        // Act

        var unrealPackage = testSerializer.Deserialize(inputTest);

        // Assert

        var names = unrealPackage.NameTable;
        names.Should().HaveCount(14);
        names.First().Name.Should().Be("A");
        names[1].Name.Should().Be("ArrayProperty");

        var imports = unrealPackage.ImportTable;
        imports.Should().HaveCount(2);
        imports[0].ClassPackage.NameIndex.Should().Be(4);
        imports[1].ClassPackage.NameIndex.Should().Be(4);

        var exports = unrealPackage.ExportTable;
        exports.Should().HaveCount(2);
        exports[0].SerialSize.Should().Be(12);
        exports[1].SerialSize.Should().Be(120);
    }


    [Fact]
    public void SerializeTest_SerializeUDKTestFile_AllExportsHasExportTableItem()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        var inputTest = File.OpenRead(@"TestData/UDK/UDKTestPackage.upk");
        serviceColection.UseSerializers(typeof(UnrealPackageSerializer), new SerializerOptions());
        var services = serviceColection.BuildServiceProvider();
        var testSerializer = services.GetRequiredService<IStreamSerializer<UnrealPackage>>();

        var importResolver = Substitute.For<IPackageCache>();
        var nativeClassFactory = new NativeClassFactory();
        // Act
        var unrealPackage = UnrealPackage.DeserializeAndInitialize(inputTest,
            new UnrealPackageOptions(testSerializer, "Core", nativeClassFactory,
                importResolver));

        importResolver.ResolveExportPackage("Core").Returns(unrealPackage);
        unrealPackage.GraphLink();
        var exportTable = unrealPackage.ExportTable;
        // Assert

        exportTable.Should().AllSatisfy(x => { x.Object.ExportTableItem.Should().NotBeNull(); });
    }

    [Fact]
    public void SerializeTest_SerializeCore_AllExportsHasExportTableItem()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        var inputTest = File.OpenRead(@"TestData/UDK/Core.u");
        serviceColection.UseSerializers(typeof(UnrealPackageSerializer), new SerializerOptions());
        var services = serviceColection.BuildServiceProvider();
        var testSerializer = services.GetRequiredService<IStreamSerializer<UnrealPackage>>();
        // Act

        var nativeClassFactory = new NativeClassFactory();
        var packageCache = new PackageCache(new PackageCacheOptions(testSerializer, nativeClassFactory));
        var unrealPackage = UnrealPackage.DeserializeAndInitialize(inputTest,
            new UnrealPackageOptions(testSerializer, "Core", nativeClassFactory,
                packageCache));
        packageCache.AddPackage(unrealPackage);
        unrealPackage.GraphLink();
        var exportTable = unrealPackage.ExportTable;
        // Assert

        exportTable.Should().AllSatisfy(x => { x.Object.ExportTableItem.Should().NotBeNull(); });
    }

    [Fact]
    public void SerializeTest_SerializEngine_AllExportsHasExportTableItem()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        var inputTest = File.OpenRead(@"TestData/UDK/Engine.u");
        serviceColection.UseSerializers(typeof(UnrealPackageSerializer), new SerializerOptions());
        var services = serviceColection.BuildServiceProvider();
        var testSerializer = services.GetRequiredService<IStreamSerializer<UnrealPackage>>();

        var nativeClassFactory = new NativeClassFactory();
        var importResolver = Substitute.For<IPackageCache>();
        var corePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Core.u"),
            new UnrealPackageOptions(testSerializer, "Core", nativeClassFactory, importResolver));
        importResolver.ResolveExportPackage("Core").Returns(corePackage);
        // Act

        var unrealPackage =
            UnrealPackage.DeserializeAndInitialize(inputTest, new UnrealPackageOptions(testSerializer, "Engine", nativeClassFactory, importResolver));
        importResolver.ResolveExportPackage("Engine").Returns(unrealPackage);
        unrealPackage.GraphLink();
        var exportTable = unrealPackage.ExportTable;
        // Assert

        exportTable.Should().AllSatisfy(x => { x.Object.ExportTableItem.Should().NotBeNull(); });
    }
}