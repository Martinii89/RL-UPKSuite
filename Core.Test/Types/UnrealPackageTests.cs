using System.IO;
using System.Linq;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Test.TestUtilities;
using Core.Utility;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Core.Types.Tests;

public class UnrealPackageTests : SerializerTestBase
{
    private readonly IStreamSerializerFor<UnrealPackage> _serializer;
    private readonly ITestOutputHelper _testOutputHelper;

    public UnrealPackageTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _serializer = GetSerializersCollection(typeof(UnrealPackage)).GetRequiredService<IStreamSerializerFor<UnrealPackage>>();
    }

    [Fact]
    public void DeserializeTest_DeserializeEncryptedPackage_ValidReturn()
    {
        // Arrange
        var inputTest = File.OpenRead("TestData/RocketPass_Premium_T_SF.upk");
        var outputStream = new MemoryStream();
        var decryptionProvider = new DecryptionProvider("keys.txt");

        var unpacked = new PackageUnpacker(inputTest, outputStream, decryptionProvider);
        //var importResolver = new ImportResolver(new ImportResolverOptions() { Extensions = { "*.u", "*.upk" }, SearchPaths = { @"D:\UDK\Custom\UDKGame\Script" } });
        var unrealPackage = new UnrealPackage();
        outputStream.Position = 0;
        // Act

        unrealPackage.Deserialize(outputStream);

        // Assert 
        var names = unrealPackage.NameTable;
        names.Count.Should().Be(unpacked.FileSummary.NameCount);
        names.First().Name.Should().Be("ArrayProperty");

        var imports = unrealPackage.ImportTable;
        imports.Count.Should().Be(unpacked.FileSummary.ImportCount);
        imports.First().ClassPackage.NameIndex.Should().Be(2);
        imports[5].ObjectName.NameIndex.Should().Be(12);

        var exports = unrealPackage.ExportTable;
        exports.Count.Should().Be(unpacked.FileSummary.ExportCount);
        exports[0].SerialSize.Should().Be(44);
        exports[1].SerialSize.Should().Be(12);

        var exportNames = unrealPackage.GetExportNamesAndOuters();

        _testOutputHelper.WriteLine(string.Join("\n", exportNames));
    }

    [Fact]
    public void FindClass_FindCoreClass_ReturnsNotNull()
    {
        // Arrange
        var packageStream = File.OpenRead(@"TestData/UDK/Core.u");
        var package = _serializer.Deserialize(packageStream);
        // Act

        var @class = package.FindClass("Class");

        // Assert 
        @class.Should().NotBeNull();
        @class.Name.Should().Be("Class");
        @class.Should().BeSameAs(package.ImportTable[3]);
    }

    [Fact]
    public void FindClass_FindComponentClass_ReturnsNotNull()
    {
        // Arrange
        var packageStream = File.OpenRead(@"TestData/UDK/Core.u");
        var package = _serializer.Deserialize(packageStream);
        // Act

        var @class = package.FindClass("Component");

        // Assert 
        @class.Should().NotBeNull();
        @class.Name.Should().Be("Component");
        @class.Should().BeSameAs(package.ExportTable[2]);
    }

    [Fact]
    public void FindClass_FindPackageClass_ReturnsNotNull()
    {
        // Arrange
        var packageStream = File.OpenRead(@"TestData/UDK/Core.u");
        var package = _serializer.Deserialize(packageStream);
        // Act

        var @class = package.FindClass("Package");

        // Assert 
        @class.Should().NotBeNull();
        @class.Name.Should().Be("Package");
    }

    [Theory]
    [InlineData("Package")]
    [InlineData("Class")]
    [InlineData("ArrayProperty")]
    [InlineData("BoolProperty")]
    [InlineData("ClassProperty")]
    public void CanCreateNativeCoreClasses(string nativeClassName)
    {
        // Arrange
        var packageStream = File.OpenRead(@"TestData/UDK/Core.u");
        var package = _serializer.Deserialize(packageStream);
        // Act

        var @class = package.FindClass(nativeClassName);

        // Assert 
        @class.Should().NotBeNull();
        @class.Name.Should().Be(nativeClassName);
    }

    [Theory]
    [InlineData("Object")]
    [InlineData("Field")]
    [InlineData("Struct")]
    [InlineData("Function")]
    [InlineData("Property")]
    [InlineData("BoolProperty")]
    [InlineData("ByteProperty")]
    [InlineData("QWordProperty")]
    [InlineData("IntProperty")]
    [InlineData("FloatProperty")]
    [InlineData("StrProperty")]
    [InlineData("NameProperty")]
    [InlineData("DelegateProperty")]
    [InlineData("ObjectProperty")]
    [InlineData("ClassProperty")]
    [InlineData("InterfaceProperty")]
    [InlineData("StructProperty")]
    [InlineData("ArrayProperty")]
    [InlineData("MapProperty")]
    [InlineData("Enum")]
    [InlineData("Const")]
    [InlineData("ScriptStruct")]
    [InlineData("State")]
    [InlineData("Class")]
    public void CorePackage_InitializesNativeClasses(string className)
    {
        // Arrange
        var packageStream = File.OpenRead(@"TestData/UDK/Core.u");
        var package = _serializer.Deserialize(packageStream);
        // Act

        var @class = package.FindClass(className);

        // Assert 
        @class.Should().NotBeNull();
        @class.Name.Should().Be(className);
        @class.Outer.Should().NotBeNull();
        @class.Outer.Name.Should().Be("Core");
        @class.Class.Should().NotBeNull();
        @class.Class.Name.Should().Be("Class");
        _testOutputHelper.WriteLine($"Initialized {package.PackageClasses.Count} classes from Core");
        _testOutputHelper.WriteLine($"Initialized {string.Join(',', package.PackageClasses.Select(x => x.Name))}");
    }

    [Fact]
    public void CreateImport_CorePackage_ResolvesCorrectly()
    {
        // Arrange
        var packageStream = File.OpenRead(@"TestData/UDK/CustomGame.u");
        var package = _serializer.Deserialize(packageStream);
        var importResolver = new ImportResolver(new ImportResolverOptions(_serializer)
            { Extensions = { "*.u", "*.upk" }, SearchPaths = { @"D:\UDK\Custom\UDKGame\Script" } });
        package.ImportResolver = importResolver;
        // Act

        var obj = package.CreateImport(package.ImportTable[3]);

        // Assert 
        obj.Should().NotBeNull();
    }
}