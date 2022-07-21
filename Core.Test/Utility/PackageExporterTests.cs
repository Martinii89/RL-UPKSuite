using System;
using System.IO;
using Core.Serialization;
using Core.Test.TestUtilities;
using Core.Types;
using Core.Types.PackageTables;
using FluentAssertions;
using NSubstitute;
using Syroot.BinaryData;
using Xunit;

namespace Core.Utility.Tests;

public class PackageExporterTests
{
    private readonly IStreamSerializer<ExportTableItem> _exportTableItemSerializer;
    private readonly IStreamSerializer<FileSummary> _headerserializer;
    private readonly IStreamSerializer<ImportTableItem> _importTableItemSerializer;
    private readonly IStreamSerializer<NameTableItem> _nameTableItemSerializer;
    private readonly UnrealPackage _testPackage;

    public PackageExporterTests()
    {
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        var nativeFactory = new NativeClassFactory();
        var options = new PackageCacheOptions(serializer, nativeFactory) { SearchPaths = { @"TestData/UDK/" }, GraphLinkPackages = false };
        var packageCache = new PackageCache(options);
        var loader = new PackageLoader(serializer, packageCache, new NeverUnpackUnpacker(), nativeFactory);
        loader.LoadPackage("TestData/UDK/UDKTestPackage.upk", "UDKTestPackage");
        _testPackage = loader.GetPackage("UDKTestPackage") ?? throw new InvalidOperationException();
        _headerserializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary));
        _nameTableItemSerializer = SerializerHelper.GetSerializerFor<NameTableItem>(typeof(NameTableItem));
        _importTableItemSerializer = SerializerHelper.GetSerializerFor<ImportTableItem>(typeof(ImportTableItem));
        _exportTableItemSerializer = SerializerHelper.GetSerializerFor<ExportTableItem>(typeof(ExportTableItem));
    }


    [Fact]
    public void ExportHeader_StreamShouldBeSerializableToEqualStruct()
    {
        // Arrange
        var stream = new MemoryStream();
        var sut = GetTestPackageExporter(stream);

        // Act
        sut.ExportHeader();
        stream.Position = 0;
        var serializedHeader = _headerserializer.Deserialize(stream);
        // Assert

        serializedHeader.Should().BeEquivalentTo(_testPackage.Header, option => option.Excluding(o => o.ThumbnailTableOffset));
        serializedHeader.ThumbnailTableOffset.Should().Be(0);
    }

    [Fact]
    public void ExportNameTable_StreamShouldBeSerializableToEqualStruct()
    {
        // Arrange
        var stream = new MemoryStream();
        var sut = GetTestPackageExporter(stream);

        // Act
        sut.ExportNameTable();
        stream.Position = 0;
        var serializedNameTable = _nameTableItemSerializer.ReadTArrayToList(stream, _testPackage.Header.NameCount);
        // Assert

        serializedNameTable.Should().BeEquivalentTo(_testPackage.NameTable);
    }

    [Fact]
    public void ExportimportTable_StreamShouldBeSerializableToEqualStruct()
    {
        // Arrange
        var stream = new MemoryStream();
        var sut = GetTestPackageExporter(stream);

        // Act
        sut.ExportImportTable();
        stream.Position = 0;
        var serializedImportTable = _importTableItemSerializer.ReadTArrayToList(stream, _testPackage.Header.ImportCount);
        // Assert

        serializedImportTable.Should().BeEquivalentTo(_testPackage.ImportTable, options => options.Excluding(o => o.ImportedObject));
    }

    [Fact]
    public void ExportExportTable_StreamShouldBeSerializableToEqualStruct()
    {
        // Arrange
        var stream = new MemoryStream();
        var sut = GetTestPackageExporter(stream);

        // Act
        sut.ExporExporttTable();
        stream.Position = 0;
        var serializedExportTable = _exportTableItemSerializer.ReadTArrayToList(stream, _testPackage.Header.ExportCount);
        // Assert

        serializedExportTable.Should().BeEquivalentTo(_testPackage.ExportTable, options => options.Excluding(o => o.Object));
    }

    [Fact]
    public void ExportDependsTable_ShouldWriteAZeroToStreamForEachImport()
    {
        // Arrange
        var stream = Substitute.For<Stream>();
        var sut = GetTestPackageExporter(stream);

        // Act
        sut.ExportDummyDependsTable();
        // Assert
        stream.Received(_testPackage.Header.ImportCount).WriteInt32(0);
    }

    [Fact]
    public void ExportThumbnailTable_ShoulNotDoAnything()
    {
        // Arrange
        var stream = new MemoryStream();
        var sut = GetTestPackageExporter(stream);

        // Act
        sut.ExportDummyThumbnailsTable();
        // Assert
        stream.Position.Should().Be(0);
    }

    private PackageExporter GetTestPackageExporter(Stream stream)
    {
        return new PackageExporter(_testPackage, stream, _headerserializer, _nameTableItemSerializer, _importTableItemSerializer, _exportTableItemSerializer);
    }
}