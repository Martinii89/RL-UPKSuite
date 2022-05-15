using System.IO;
using System.Linq;
using Core.Serialization.RocketLeague;
using Core.Types;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
        var testSerializer = services.GetRequiredService<IStreamSerializerFor<UnrealPackage>>();
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
        var testSerializer = services.GetRequiredService<IStreamSerializerFor<UnrealPackage>>();
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
        var testSerializer = services.GetRequiredService<IStreamSerializerFor<UnrealPackage>>();
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
        var testSerializer = services.GetRequiredService<IStreamSerializerFor<UnrealPackage>>();
        // Act

        var unrealPackage = testSerializer.Deserialize(inputTest);
        unrealPackage.GraphLink();
        var exportTable = unrealPackage.ExportTable;
        // Assert

        exportTable.Should().AllSatisfy(x => { x.Object.ExportTableItem.Should().NotBeNull(); });
    }
}