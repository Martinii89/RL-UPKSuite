using System;
using System.IO;
using Core.Serialization;
using Core.Test.TestUtilities;
using Core.Types;
using Core.Types.PackageTables;
using FluentAssertions;
using Xunit;

namespace Core.Utility.Tests;

public class PackageExporterTests
{
    private readonly IStreamSerializer<FileSummary> _headerserializer;
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

        serializedHeader.Should().BeEquivalentTo(_testPackage.Header);
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
        var serializedHeader = _nameTableItemSerializer.ReadTArrayToList(stream, _testPackage.Header.NameCount);
        // Assert

        serializedHeader.Should().BeEquivalentTo(_testPackage.NameTable);
    }

    private PackageExporter GetTestPackageExporter(Stream stream)
    {
        return new PackageExporter(_testPackage, stream, _headerserializer, _nameTableItemSerializer);
    }
}