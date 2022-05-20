using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Classes.Core;
using Core.RocketLeague;
using Core.RocketLeague.Decryption;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Serialization.RocketLeague;
using Core.Test.TestUtilities;
using Core.Utility;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Core.Types.Tests;

public class UnrealPackageTests : SerializerHelper, IClassFixture<PackageStreamFixture>
{
    private readonly IStreamSerializerFor<FileSummary> _fileSummarySerializer;
    private readonly PackageStreamFixture _packageStreams;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IStreamSerializerFor<UnrealPackage> _udkPackageSerializer;

    public UnrealPackageTests(ITestOutputHelper testOutputHelper, PackageStreamFixture packageStreams)
    {
        _testOutputHelper = testOutputHelper;
        _packageStreams = packageStreams;
        _udkPackageSerializer = GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        _fileSummarySerializer = GetSerializerFor<FileSummary>(typeof(UnrealPackage));
    }

    [Fact]
    public void DeserializeTest_DeserializeEncryptedPackage_ValidReturn()
    {
        // Arrange
        var inputTest = File.OpenRead("TestData/RocketPass_Premium_T_SF.upk");
        var outputStream = new MemoryStream();
        var decryptionProvider = new DecryptionProvider("keys.txt");

        var packedFile = new RLPackageUnpacker(inputTest, decryptionProvider, _fileSummarySerializer);
        packedFile.Unpack(outputStream);
        outputStream.Position = 0;

        var serializer = GetSerializerFor<UnrealPackage>(typeof(UnrealPackage), RocketLeagueBase.FileVersion);
        // Act
        var unrealPackage = serializer.Deserialize(outputStream);
        //unrealPackage.Deserialize(outputStream);

        // Assert 
        var names = unrealPackage.NameTable;
        names.Count.Should().Be(packedFile.FileSummary.NameCount);
        names.First().Name.Should().Be("ArrayProperty");

        var imports = unrealPackage.ImportTable;
        imports.Count.Should().Be(packedFile.FileSummary.ImportCount);
        imports.First().ClassPackage.NameIndex.Should().Be(2);
        imports[5].ObjectName.NameIndex.Should().Be(12);

        var exports = unrealPackage.ExportTable;
        exports.Count.Should().Be(packedFile.FileSummary.ExportCount);
        exports[0].SerialSize.Should().Be(44);
        exports[1].SerialSize.Should().Be(12);
    }

    [Fact]
    public void FindClass_FindCoreClass_ReturnsNotNull()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");
        // Act

        var @class = package.FindClass("Class");

        // Assert 
        @class.Should().NotBeNull();
        @class.Name.Should().Be("Class");
        @class.Should().BeSameAs(package.ImportTable[3].ImportedObject);
    }

    [Fact]
    public void FindClass_FindComponentClass_ReturnsNotNull()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.GraphLink();
        // Act
        var @class = package.FindClass("Component");

        // Assert 
        @class.Should().NotBeNull();
        @class.Name.Should().Be("Component");
        @class.Should().BeSameAs(package.ExportTable[2].Object);
    }

    [Fact]
    public void FindClass_FindPackageClass_ReturnsNotNull()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");
        // Act

        var @class = package.FindClass("Package");

        // Assert 
        @class.Should().NotBeNull();
        @class.Name.Should().Be("Package");
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
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");
        // Act

        var @class = package.FindClass(className);

        // Assert 
        @class.Should().NotBeNull();
        @class!.Name.Should().Be(className);
        @class.Outer.Should().NotBeNull();
        @class.Outer!.Name.Should().Be("Core");
        @class.Class.Should().NotBeNull();
        @class.Class!.Name.Should().Be("Class");
        _testOutputHelper.WriteLine($"Initialized {package.PackageClasses.Count} classes from Core");
        _testOutputHelper.WriteLine($"Initialized {string.Join(',', package.PackageClasses.Select(x => x.Name))}");
    }

    [Fact]
    public void CorePackage_InitializesNativeClasses_NoDuplicates()
    {
        // Arrange
        // Act

        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);

        // Assert 
        package.PackageClasses.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void CorePackage_InitializesUClassStaticClass()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");
        // Act

        // Assert 
        UClass.StaticClass.Should().NotBeNull();
    }

    [Fact]
    public void LinkImports_CorePackage_AllImportsCreated()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");

        // Act


        // Assert 
        package.ImportTable.Should().OnlyContain(item => item.ImportedObject != null);
    }

    [Fact]
    public void LinkImports_CorePackage_AllImportsLinked()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");

        // Act

        var importObjects = package.ImportTable.Select(x => x.ImportedObject).ToList();

        // Assert 
        importObjects.Should().AllSatisfy(x =>
        {
            x.Should().NotBeNull();
            x!.Class.Should().NotBeNull();
            if (x != package.PackageRoot)
            {
                x.Outer.Should().NotBeNull();
            }
        });
    }

    [Fact]
    public void LinkExports_CorePackage_AllExportsCreated()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);

        // Act

        package.CreateExportObjects();

        // Assert 
        package.ExportTable.Should().OnlyContain(item => item.Object != null);
    }

    [Fact]
    public void GraphLink_CorePackage_AllExportsLinked()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");

        // Act

        package.GraphLink();
        //package.LinkExports();

        // Assert 
        package.ExportTable.Should().AllSatisfy(x =>
        {
            var obj = x.Object;
            obj.Class.Should().NotBeNull();
            if (x.OuterIndex.Index != 0)
            {
                obj.Outer.Should().NotBeNull();
            }

            if (x.ArchetypeIndex.Index != 0)
            {
                obj.ObjectArchetype.Should().NotBeNull();
            }

            obj.Outer.Should().NotBeNull();
        });
    }

    [Fact]
    public void GetFullName_CorePackage_AllImportsStartsWithCore()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");
        // Act

        var fullNames = package.ImportTable.Select(package.GetFullName);

        // Assert 
        fullNames.Should().OnlyContain(s => s.StartsWith("Core"));
    }

    [Fact]
    public void GetFullName_CorePackage_CoreClass()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");
        // Act

        var coreObjectFullName = package.GetFullName(package.ImportTable[3]);

        // Assert 
        coreObjectFullName.Should().Be("Core.Class");
    }

    [Fact]
    public void GetFullName_CorePackage_CoreObject()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");
        // Act

        var coreObjectFullName = package.GetFullName(package.ExportTable[0]);

        // Assert 
        coreObjectFullName.Should().Be("Core.Object");
    }

    [Fact]
    public void GetFullName_CorePackage_AllExportsStartsWithCore()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CoreStream);
        package.PostDeserializeInitialize("Core");
        // Act

        var fullNames = package.ExportTable.Select(package.GetFullName);

        // Assert 
        fullNames.Should().OnlyContain(s => s.StartsWith("Core"));
    }

    [Fact]
    public void CreateImport_CorePackage_ClassResolvesCorrectly()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CustomGameStream);
        package.PostDeserializeInitialize("CustomGame");
        var importResolver = new PackageCache(new PackageCacheOptions(_udkPackageSerializer)
            { Extensions = { "*.u", "*.upk" }, SearchPaths = { @"TestData/UDK" } });
        package.ImportResolver = importResolver;
        // Act

        var obj = package.CreateImport(package.ImportTable[1]);

        // Assert 
        obj.Should().NotBeNull();
    }

    [Fact]
    public void CreateImport_CorePackage_ObjectResolvesCorrectly()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CustomGameStream);
        package.PostDeserializeInitialize("CustomGame");
        var importResolver = new PackageCache(new PackageCacheOptions(_udkPackageSerializer)
            { Extensions = { "*.u", "*.upk" }, SearchPaths = { @"TestData/UDK" } });
        package.ImportResolver = importResolver;
        // Act

        var obj = package.CreateImport(package.ImportTable[2]);

        // Assert 
        obj.Should().NotBeNull();
    }

    [Fact]
    public void CreateImport_CorePackage_StateResolvesCorrectly()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CustomGameStream);
        package.PostDeserializeInitialize("CustomGame");
        var importResolver = new PackageCache(new PackageCacheOptions(_udkPackageSerializer)
            { Extensions = { "*.u", "*.upk" }, SearchPaths = { @"TestData/UDK" } });
        package.ImportResolver = importResolver;
        // Act

        var obj = package.CreateImport(package.ImportTable[3]);

        // Assert 
        obj.Should().NotBeNull();
    }

    [Fact]
    public void CreateImport_CorePackage_AllCoreImportsResolves()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.CustomGameStream);
        package.PostDeserializeInitialize("CustomGame");
        var importResolver = new PackageCache(new PackageCacheOptions(_udkPackageSerializer)
            { Extensions = { "*.u", "*.upk" }, SearchPaths = { @"TestData/UDK" } });
        package.ImportResolver = importResolver;

        var coreImports = package.ImportTable.Where(x => package.GetFullName(x).StartsWith("Core.")).ToList();

        // Act

        var imports = coreImports.Select(package.CreateImport);

        // Assert 
        imports.Should().AllSatisfy(x => { x.Should().NotBeNull(); });
    }

    [Fact]
    public void CreateImport_EnginePackage_NativeClassesInjected()
    {
        // Arrange
        var packageImportResolver = Substitute.For<IPackageCache>();
        var loadOptions = new UnrealPackageOptions(_udkPackageSerializer, "Core", packageImportResolver);
        var corePackage = UnrealPackage.DeserializeAndInitialize(_packageStreams.CoreStream, loadOptions);
        packageImportResolver.ResolveExportPackage("Core").Returns(corePackage);


        var engineNativeClasses = new List<string>
            { "ChildConnection", "Client", "FracturedStaticMesh", "Level", "Model", "NetConnection", "PendingLevel", "ShadowMap1D", "StaticMesh" };

        // Act
        var enginePackage = UnrealPackage.DeserializeAndInitialize(_packageStreams.EngineStream,
            new UnrealPackageOptions(_udkPackageSerializer, "Engine", packageImportResolver));
        packageImportResolver.ResolveExportPackage("Engine").Returns(enginePackage);

        var classes = engineNativeClasses.Select(enginePackage.FindClass);

        // Assert 
        classes.Should().AllSatisfy(x => { x.Should().NotBeNull(); });
    }

    [Fact]
    public void CreateImport_EnginePackage_AllCoreImportsResolves()
    {
        // Arrange
        var package = _udkPackageSerializer.Deserialize(_packageStreams.EngineStream);
        var importResolver = new PackageCache(new PackageCacheOptions(_udkPackageSerializer)
            { Extensions = { "*.u", "*.upk" }, SearchPaths = { @"TestData/UDK" } });
        package.ImportResolver = importResolver;
        package.PostDeserializeInitialize("Engine");

        var coreImports = package.ImportTable.Where(x => package.GetName(package.GetImportPackage(x).ObjectName) == "Core").ToList();

        // Act
        coreImports.ForEach(x => package.CreateImport(x));
        var importObjects = coreImports.Select(x => x.ImportedObject);

        // Assert 
        importObjects.Should().AllSatisfy(x => { x.Should().NotBeNull(); });
    }

    [Fact]
    public void CreateImport_CorePackage_AllClassAndObjectSerializersNonNull()
    {
        // Arrange
        var serializerFactory = GetService<IObjectSerializerFactory>(typeof(UnrealPackage));
        var package = UnrealPackage.DeserializeAndInitialize(_packageStreams.CoreStream,
            new UnrealPackageOptions(_udkPackageSerializer, "Core", null, serializerFactory));
        package.GraphLink();

        // Act
        var classes = package.PackageClasses;
        var objects = package.ExportTable.Select(x => x.Object);

        // Assert 
        classes.Should().AllSatisfy(x => { x.GetInstanceSerializer().Should().NotBeNull(); });
        objects.Should().AllSatisfy(x => { x.Serializer.Should().NotBeNull(); });
    }
}