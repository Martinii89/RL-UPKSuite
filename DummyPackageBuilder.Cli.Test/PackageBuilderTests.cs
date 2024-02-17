using Core;
using Core.Serialization;
using Core.Serialization.Abstraction;
using Core.Test.TestUtilities;
using Core.Types;
using Core.Types.PackageTables;
using Core.Utility;
using Core.Utility.Export;

using FluentAssertions;

namespace DummyPackageBuilder.Test;

public class PackageBuilderTests
{
    private const string PackageName = "MyTestPackage";

    private readonly DummyPackageBuilderCli.DummyPackageBuilder _builder;

    private readonly IStreamSerializer<ExportTableItem> _exportTableItemSerializer;

    private readonly IStreamSerializer<FName> _fNameSerializer;

    private readonly IStreamSerializer<FileSummary> _headerSerializer;

    private readonly IStreamSerializer<ImportTableItem> _importTableItemSerializer;

    private readonly IStreamSerializer<NameTableItem> _nameTableItemSerializer;

    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;

    private readonly IObjectSerializerFactory _udkObjectSerializerFactory;

    public PackageBuilderTests()
    {
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        NativeClassFactory nativeFactory = new();
        IObjectSerializerFactory objectSerializerFactory = SerializerHelper.GetService<IObjectSerializerFactory>(typeof(IObjectSerializerFactory));
        var options = new PackageCacheOptions(serializer, nativeFactory)
        {
            SearchPaths =
            {
                "TestData/UDK/"
            },
            GraphLinkPackages = true,
            ObjectSerializerFactory = objectSerializerFactory
        };
        var packageCache = new PackageCache(options);
        _builder = new DummyPackageBuilderCli.DummyPackageBuilder(PackageName)
            .WithPackageCache(packageCache)
            .WithNativeFactory(nativeFactory);

        _headerSerializer = SerializerHelper.GetSerializerFor<FileSummary>(typeof(FileSummary));
        _nameTableItemSerializer = SerializerHelper.GetSerializerFor<NameTableItem>(typeof(NameTableItem));
        _importTableItemSerializer = SerializerHelper.GetSerializerFor<ImportTableItem>(typeof(ImportTableItem));
        _exportTableItemSerializer = SerializerHelper.GetSerializerFor<ExportTableItem>(typeof(ExportTableItem));
        _fNameSerializer = SerializerHelper.GetSerializerFor<FName>(typeof(FName));
        _objectIndexSerializer = SerializerHelper.GetSerializerFor<ObjectIndex>(typeof(ObjectIndex));
        _udkObjectSerializerFactory =
            SerializerHelper.GetService<IObjectSerializerFactory>(typeof(IObjectSerializerFactory));
    }

    [Fact(Skip = "just for inspecting the package data")]
    public void LoadPackageTest_LoadDummyPackage()
    {
        // Arrange
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        var nativeFactory = new NativeClassFactory();
        var options = new PackageCacheOptions(serializer, nativeFactory)
        {
            SearchPaths =
            {
                @"TestData/UDK/"
            },
            GraphLinkPackages = true
        };
        var packageCache = new PackageCache(options);
        var loader = new PackageLoader(serializer, packageCache, new NeverUnpackUnpacker(), nativeFactory);

        // Act
        loader.LoadPackage($"TestData/UDK/DummyPackages/FXActorEvents.upk", "FXActorEvents");
        var package = loader.GetPackage("FXActorEvents");
        // Assert 
        package!.ExportTable.Should().AllSatisfy(x => { x.Object.Should().NotBeNull(); });
        package.ImportTable.Should().AllSatisfy(x => { x.ImportedObject.Should().NotBeNull(); });
    }

    [Fact]
    public void BuildPackage_SetsPackageName()
    {
        var pck = _builder.BuildPackage();
        // Assert
        pck.PackageName.Should().Be(PackageName);
    }

    [Fact]
    public void BuildPackage_ContainsMinimalNames()
    {
        // Arrange
        _builder.SkipGraphLinkPackage();

        // Act
        UnrealPackage pck = _builder.BuildPackage();

        // Assert

        var names = pck.NameTable.Select(x => x.Name).ToList();
        names.Should().Contain(["Class", "Core", "None", "Package"]);
    }

    [Fact]
    public void AddObject_OfTypeFXActorEvent_X_AddsImportsForPackageAndClass()
    {
        // Act
        _builder.AddObject(null, "MyObj", "projectx.FXActorEvent_X");
        var pck = _builder.BuildPackage();

        // Assert
        var names = pck.NameTable.Select(x => x.Name).ToList();
        names.Should().Contain(["projectx", "FXActorEvent_X"]);

        var imports = pck.ImportTable.ToList();
        var packageImport = imports.FirstOrDefault(x => pck.GetName(x.ObjectName) == "projectx");
        pck.GetName(packageImport!.ClassName).Should().Be("Package");
        pck.GetName(packageImport.ClassPackage).Should().Be("Core");

        var classImport = imports.FirstOrDefault(x => pck.GetName(x.ObjectName) == "FXActorEvent_X");
        pck.GetName(classImport!.ClassPackage).Should().Be("Core");
        pck.GetName(classImport.ClassName).Should().Be("Class");
        pck.GetObject(classImport.OuterIndex)!.Name.Should().Be("projectx");

        packageImport.Should().NotBeNull();
        classImport.Should().NotBeNull();
    }

    [Fact]
    public void AddObject_CreatesExportsWithCorrectMetadata()
    {
        // Act
        _builder.AddObject(null, "MyObj", "projectx.FXActorEvent_X");
        _builder.SkipGraphLinkPackage();
        var pck = _builder.BuildPackage();

        // Assert

        pck.ExportTable.Should().HaveCount(1);
        var myObj = pck.ExportTable[0];
        IObjectResource? classReference = pck.GetObjectReference(myObj.ClassIndex);
        pck.GetName(classReference!.ObjectName).Should().Be("FXActorEvent_X");
        myObj.SuperIndex.Index.Should().Be(0);
        myObj.OuterIndex.Index.Should().Be(0);

        myObj.ArchetypeIndex.Index.Should().Be(0);
        myObj.SerialSize.Should().Be(-1);
    }

    [Fact]
    public void AddObject_WhenObjectIsNested_AddsPackageImport()
    {
        // Act
        _builder.AddObject("MyGroup", "MyObj", "projectx.FXActorEvent_X");
        _builder.SkipGraphLinkPackage();

        var pck = _builder.BuildPackage();

        // Assert
        var corePackage = pck.ImportTable.FirstOrDefault(x =>
            pck.GetName(x.ObjectName) == "Core" && x.OuterIndex.Index == 0 && pck.GetName(x.ClassName) == "Package");
        corePackage.Should().NotBeNull();
        var packageClassImport = pck.ImportTable.FirstOrDefault(x =>
            pck.GetName(x.ObjectName) == "Package"
            && x.OuterIndex.Index != 0
            && pck.GetName(x.ClassPackage) == "Core"
            && pck.GetName(x.ClassName) == "Class");

        packageClassImport.Should().NotBeNull();
    }

    [Fact]
    public void AddObject_WhenObjectIsNested_CreatesTheRightStructure()
    {
        // Act
        _builder.AddObject("MyGroup", "MyObj", "projectx.FXActorEvent_X");
        _builder.SkipGraphLinkPackage();

        var pck = _builder.BuildPackage();

        // Assert

        pck.ExportTable.Should().HaveCount(2);
        var myObj = pck.ExportTable.FirstOrDefault(x => pck.GetName(x.ObjectName) == "MyObj");
        myObj.Should().NotBeNull();

        IObjectResource? classReference = pck.GetObjectReference(myObj!.ClassIndex);
        pck.GetName(classReference!.ObjectName).Should().Be("FXActorEvent_X");
        myObj.SuperIndex.Index.Should().Be(0);
        myObj.OuterIndex.Index.Should().NotBe(0);

        myObj.ArchetypeIndex.Index.Should().Be(0);
        myObj.SerialSize.Should().Be(-1);
    }

    [Fact]
    public void AddObject_WhenObjectIsMultipleNested_CreatesTheRightStructure()
    {
        // Act
        _builder.AddObject("MyGroup.MyGroup2", "MyObj", "projectx.FXActorEvent_X");
        _builder.SkipGraphLinkPackage();

        var pck = _builder.BuildPackage();

        // Assert

        pck.ExportTable.Should().HaveCount(3);
        var myObj = pck.ExportTable.FirstOrDefault(x => pck.GetName(x.ObjectName) == "MyObj");
        myObj.Should().NotBeNull();

        var packages = pck.ExportTable.Where(x => x != myObj).ToList();
        packages.Should().HaveCount(2);

        packages.Should().AllSatisfy(x =>
        {
            IObjectResource? classReference = pck.GetObjectReference(x.ClassIndex);
            pck.GetName(classReference!.ObjectName).Should().Be("Package");
        });
    }

    [Fact]
    public void AddObject_TwoObjectsInSameGroup_CreatesTheRightStructure()
    {
        // Arrange
        _builder.AddObject("MyGroup", "MyObj", "projectx.FXActorEvent_X");
        _builder.AddObject("MyGroup", "MyObj2", "projectx.FXActorEvent_X");
        _builder.SkipGraphLinkPackage();

        var pck = _builder.BuildPackage();

        var groupIndex = pck.ExportTable.FindIndex(x => pck.GetName(x.ObjectName) == "MyGroup");
        groupIndex.Should().NotBe(-1);

        var objects = pck.ExportTable.Where(x => pck.GetName(x.ObjectName) != "MyGroup").ToList();
        objects.Should().HaveCount(2);

        objects.Should().AllSatisfy(o =>
        {
            o.OuterIndex.Index.Should().Be(ObjectIndex.FromExportIndex(groupIndex));
        });
    }

    [Fact]
    public void AddObject_OfTypeFXActorEvent_X_CreatesAExportItemWithTheRightNameAndClass()
    {
        // Act
        _builder.AddObject(null, "MyObj", "projectx.FXActorEvent_X");
        _builder.SkipGraphLinkPackage();
        var pck = _builder.BuildPackage();

        // Assert

        pck.ExportTable.Should().HaveCount(1);

        var export = pck.ExportTable.First();
        var exportName = pck.GetFullName(export);
        IObjectResource? objectReference = pck.GetObjectReference(export.ClassIndex);
        objectReference.Should().NotBeNull();
        var className = pck.GetFullName(objectReference!);

        exportName.Should().Be($"{PackageName}.MyObj");
        className.Should().Be("projectx.FXActorEvent_X");
    }

    [Fact]
    public void ExportPackage_WhenSourceIsCustomBuiltPackage_DoesNotThrow()
    {
        // Arrange
        List<string> fxActors = ["Active", "AnimationEnded", "Boost", "BoostEndEvent", "BoostFly", "BoostPreview"];
        foreach (string name in fxActors)
        {
            _builder.AddObject(null, name, "projectx.FXActorEvent_X");  
        }
        _builder.AddObject("SomeGroup", "SomeGroupedActor", "projectx.FXActorEvent_X");
        _builder.AddObject("SomeGroup", "SomeGroupedActor2", "projectx.FXActorEvent_X");
        
        var pck = _builder.BuildPackage();

        using var stream = new FileStream("MyPacakge.upk", FileMode.OpenOrCreate);
        // Act

        var exporter = GetPackageExporter(stream, pck);
        exporter.ExportPackage();
    }

    private PackageExporter GetPackageExporter(Stream stream, UnrealPackage package)
    {
        return new PackageExporter(package, stream, _headerSerializer, _nameTableItemSerializer,
            _importTableItemSerializer, _exportTableItemSerializer,
            _objectIndexSerializer, _fNameSerializer, _udkObjectSerializerFactory, skipFilters: true);
    }
}

