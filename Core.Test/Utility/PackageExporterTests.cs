using System;
using System.IO;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Serialization.RocketLeague;
using Core.Test.TestUtilities;
using Core.Types;
using Core.Types.PackageTables;
using Core.Utility.Export;
using FluentAssertions;
using NSubstitute;
using Syroot.BinaryData;
using Xunit;

namespace Core.Utility.Tests;

public class PackageExporterTests
{
    private readonly IStreamSerializer<ExportTableItem> _exportTableItemSerializer;
    private readonly IStreamSerializer<FName> _fNameSerializer;
    private readonly IStreamSerializer<FileSummary> _headerserializer;
    private readonly IStreamSerializer<ImportTableItem> _importTableItemSerializer;
    private readonly IStreamSerializer<NameTableItem> _nameTableItemSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;
    private readonly PackageLoader _packageLoader;
    private readonly UnrealPackage _testPackage;
    private readonly UnrealPackage _testPackageMaterial;

    public PackageExporterTests()
    {
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        var nativeFactory = new NativeClassFactory();
        var objectSerializerFactory = SerializerHelper.GetService<IObjectSerializerFactory>(typeof(IObjectSerializerFactory));
        var options = new PackageCacheOptions(serializer, nativeFactory)
            { SearchPaths = { @"TestData/UDK/" }, GraphLinkPackages = true, ObjectSerializerFactory = objectSerializerFactory };
        var packageCache = new PackageCache(options);
        _packageLoader = new PackageLoader(serializer, packageCache, new NeverUnpackUnpacker(), nativeFactory, objectSerializerFactory);
        _packageLoader.LoadPackage("TestData/UDK/UDKTestPackage.upk", "UDKTestPackage");
        _packageLoader.LoadPackage("TestData/UDK/TestMaterials.upk", "TestMaterials");
        _testPackage = _packageLoader.GetPackage("UDKTestPackage") ?? throw new InvalidOperationException();
        _testPackageMaterial = _packageLoader.GetPackage("TestMaterials") ?? throw new InvalidOperationException();
        _headerserializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary));
        _nameTableItemSerializer = SerializerHelper.GetSerializerFor<NameTableItem>(typeof(NameTableItem));
        _importTableItemSerializer = SerializerHelper.GetSerializerFor<ImportTableItem>(typeof(ImportTableItem));
        _exportTableItemSerializer = SerializerHelper.GetSerializerFor<ExportTableItem>(typeof(ExportTableItem));
        _fNameSerializer = SerializerHelper.GetSerializerFor<FName>(typeof(FName));
        _objectIndexSerializer = SerializerHelper.GetSerializerFor<ObjectIndex>(typeof(ObjectIndex));
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

        serializedExportTable.Should().BeEquivalentTo(_testPackage.ExportTable, options => options.Excluding(o => o.Object).Excluding(o => o.ObjectFlags));
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

    [Fact]
    public void ExportObjectSerialData_ShoulNotThrow()
    {
        // Arrange
        var stream = new MemoryStream();
        var sut = GetTestPackageExporter(stream);

        // Act
        var act = () => sut.ExportObjectSerialData();
        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ExportObjectSerialData_ExportedSerialDataShouldBeEqual()
    {
        // Arrange
        var stream = new MemoryStream();
        var sut = GetTestPackageExporter(stream);
        var packageStream = _testPackage.PackageStream;
        var serialStart = _testPackage.ExportTable[0].SerialOffset;
        var serialEnd = _testPackage.ExportTable[1].SerialOffset + _testPackage.ExportTable[1].SerialSize;
        packageStream.Position = serialStart;
        var serialData = packageStream.ReadBytes((int) (serialEnd - serialStart));
        // Act
        sut.ExportObjectSerialData();
        var exportBuffer = new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length);
        // Assert
        exportBuffer.Count.Should().Be(serialData.Length);
        exportBuffer.Should().BeEquivalentTo(serialData);
    }

    [Fact]
    public void ExportPackage_ExportedPackageData_ShouldBeParsable()
    {
        // Arrange
        var stream = new MemoryStream();
        var sut = GetTestPackageExporter(stream);
        // Act
        sut.ExportPackage();
        var exportBuffer = new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length);
        File.WriteAllBytes("TestData/UDK/UDKTestPackage_exported.upk", exportBuffer.ToArray());
        var act = () => _packageLoader.LoadPackage("TestData/UDK/UDKTestPackage_exported.upk", "UDKTestPackage_exported");
        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ExportSerialdata_WithSerializerFactory_ShouldNotThrow()
    {
        // Arrange
        var stream = new MemoryStream();
        var sut = GetTestPackageExporter(stream);
        var factory = SerializerHelper.GetService<IObjectSerializerFactory>(typeof(IObjectSerializerFactory));
        // Act
        var act = () => sut.ExportObjectSerialData(factory);
        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ExportMaterialPackage_ExportedPackageData_ShouldBeParsable()
    {
        // Arrange
        var stream = new MemoryStream();
        var sut = GetMaterialsTestPackageExporter(stream);
        // Act
        sut.ExportPackage();
        var exportBuffer = new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length);
        File.WriteAllBytes("TestData/UDK/UDKTestPackageMaterial_exported.upk", exportBuffer.ToArray());
        var act = () => _packageLoader.LoadPackage("TestData/UDK/UDKTestPackageMaterial_exported.upk", "UDKTestPackageMaterial_exported");
        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ExportRLPackage_ExportPackage_ShouldNotThrow()
    {
        // Arrange
        var stream = new MemoryStream();

        // Arrange
        var fileSummarySerializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary), RocketLeagueBase.FileVersion);
        var unpacker = new PackageUnpacker(fileSummarySerializer, new DecryptionProvider("keys.txt"));
        var packageSerializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage), RocketLeagueBase.FileVersion);
        var nativeFactory = new NativeClassFactory();
        var RLobjectSerializerFactory = SerializerHelper.GetService<IObjectSerializerFactory>(typeof(IObjectSerializerFactory), RocketLeagueBase.FileVersion);
        var options = new PackageCacheOptions(packageSerializer, nativeFactory)
        {
            SearchPaths = { @"D:\SteamLibrary\steamapps\common\rocketleague\TAGame\CookedPCConsole" }, GraphLinkPackages = true, PackageUnpacker = unpacker,
            NativeClassFactory = nativeFactory, ObjectSerializerFactory = RLobjectSerializerFactory
        };
        var packageCache = new PackageCache(options);
        var UDKobjectSerializerFactory = SerializerHelper.GetService<IObjectSerializerFactory>(typeof(IObjectSerializerFactory));
        var loader = new PackageLoader(packageSerializer, packageCache, unpacker, nativeFactory, UDKobjectSerializerFactory);
        var package = loader.LoadPackage("TestData/body_bb_SF.upk", "body_bb_SF");
        var sut = GetPackageExporter(stream, package);

        // Act
        var act = () => sut.ExportPackage(UDKobjectSerializerFactory);
        // Assert
        act.Should().NotThrow();
        var exportBuffer = new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length);
        File.WriteAllBytes("TestData/body_bb_SF_exported.upk", exportBuffer.ToArray());
    }

    [Fact]
    public void ExportRLMapPackage_ExportPackage_ShouldNotThrow()
    {
        // Arrange
        var stream = new MemoryStream();

        // Arrange
        var fileSummarySerializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary), RocketLeagueBase.FileVersion);
        var unpacker = new PackageUnpacker(fileSummarySerializer, new DecryptionProvider("keys.txt"));
        var packageSerializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage), RocketLeagueBase.FileVersion);
        var nativeFactory = new NativeClassFactory();
        var RLobjectSerializerFactory = SerializerHelper.GetService<IObjectSerializerFactory>(typeof(IObjectSerializerFactory), RocketLeagueBase.FileVersion);
        var options = new PackageCacheOptions(packageSerializer, nativeFactory)
        {
            SearchPaths = { @"D:\SteamLibrary\steamapps\common\rocketleague\TAGame\CookedPCConsole" }, GraphLinkPackages = true, PackageUnpacker = unpacker,
            NativeClassFactory = nativeFactory, ObjectSerializerFactory = RLobjectSerializerFactory
        };
        var packageCache = new PackageCache(options);
        var UDKobjectSerializerFactory = SerializerHelper.GetService<IObjectSerializerFactory>(typeof(IObjectSerializerFactory));
        var loader = new PackageLoader(packageSerializer, packageCache, unpacker, nativeFactory, UDKobjectSerializerFactory);
        var package = loader.LoadPackage("TestData/Park_P.upk", "Park_P");
        var sut = GetPackageExporter(stream, package);

        // Act
        var act = () => sut.ExportPackage(UDKobjectSerializerFactory);
        // Assert
        act.Should().NotThrow();
        var exportBuffer = new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length);
        File.WriteAllBytes("TestData/Park_P_exported.upk", exportBuffer.ToArray());
    }

    private PackageExporter GetPackageExporter(Stream stream, UnrealPackage package)
    {
        return new PackageExporter(package, stream, _headerserializer, _nameTableItemSerializer, _importTableItemSerializer, _exportTableItemSerializer,
            _objectIndexSerializer, _fNameSerializer);
    }

    private PackageExporter GetTestPackageExporter(Stream stream)
    {
        return new PackageExporter(_testPackage, stream, _headerserializer, _nameTableItemSerializer, _importTableItemSerializer, _exportTableItemSerializer,
            _objectIndexSerializer, _fNameSerializer);
    }

    private PackageExporter GetMaterialsTestPackageExporter(Stream stream)
    {
        return new PackageExporter(_testPackageMaterial, stream, _headerserializer, _nameTableItemSerializer, _importTableItemSerializer,
            _exportTableItemSerializer,
            _objectIndexSerializer, _fNameSerializer);
    }
}