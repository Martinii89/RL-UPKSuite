using FluentAssertions;

using NSubstitute;

using RlUpk.Core.Classes;
using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;
using RlUpk.Core.Utility;
using RlUpk.TestUtils.TestUtilities;

using Xunit;
using Xunit.Abstractions;

namespace RlUpk.Core.Test.Utility;

public class UdkPackages
{
    public readonly UnrealPackage Core;
    public readonly UnrealPackage CustomGame;
    public readonly UnrealPackage Engine;
    public readonly UnrealPackage GameFramework;
    public readonly IPackageCache PackageCache;

    public UdkPackages()
    {
        var core = new MemoryStream(File.ReadAllBytes(@"TestData/UDK/Core.u"));
        var engine = new MemoryStream(File.ReadAllBytes(@"TestData/UDK/Engine.u"));
        var customGame = new MemoryStream(File.ReadAllBytes(@"TestData/UDK/CustomGame.u"));
        var gameFramework = new MemoryStream(File.ReadAllBytes(@"TestData/UDK/GameFramework.u"));
        var nativeFactory = new NativeClassFactory();

        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        PackageCache = Substitute.For<IPackageCache>();

        Core = UnrealPackage.DeserializeAndInitialize(core, new UnrealPackageOptions(serializer, "Core", nativeFactory));
        PackageCache.ResolveExportPackage("Core").Returns(Core);

        Engine = UnrealPackage.DeserializeAndInitialize(engine, new UnrealPackageOptions(serializer, "Engine", nativeFactory, PackageCache));
        PackageCache.ResolveExportPackage("Engine").Returns(Engine);

        GameFramework = UnrealPackage.DeserializeAndInitialize(customGame, new UnrealPackageOptions(serializer, "GameFramework", nativeFactory, PackageCache));
        PackageCache.ResolveExportPackage("GameFramework").Returns(GameFramework);

        CustomGame = UnrealPackage.DeserializeAndInitialize(gameFramework, new UnrealPackageOptions(serializer, "CustomGame", nativeFactory, PackageCache));
        PackageCache.ResolveExportPackage("CustomGame").Returns(CustomGame);
    }
}

public class CrossPackageDependencyGraphTests : IClassFixture<UdkPackages>
{
    private readonly IPackageCache _packageCache;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly UdkPackages _udkPackages;

    public CrossPackageDependencyGraphTests(ITestOutputHelper testOutputHelper, UdkPackages udkPackages)
    {
        _testOutputHelper = testOutputHelper;
        _udkPackages = udkPackages;
        _packageCache = udkPackages.PackageCache;
    }

    [Fact]
    public void AddNodeTest_AddNode_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));

        // Act
        var action = () => graph.AddNode(node);

        // Assert 
        action.Should().NotThrow();
    }

    [Fact]
    public void AddNodeTest_AddNode_NodeCountIncreasesForNewNode()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));
        var startCount = graph.NodeCount;

        // Act
        graph.AddNode(node);

        // Assert 
        graph.NodeCount.Should().Be(startCount + 1);
    }

    [Fact]
    public void AddNodeTest_AddNode_NodeCountConstantForDuplicateNode()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));
        graph.AddNode(node);
        var startCount = graph.NodeCount;
        // Act
        graph.AddNode(node);

        // Assert 
        graph.NodeCount.Should().Be(startCount);
    }

    [Fact]
    public void GetEdgesTest_UnknownNode_ThrowsKeyNotFound()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));
        graph.AddNode(node);

        // Act

        var action = () => graph.GetEdges(new PackageObjectReference("unknown", new ObjectIndex(0)));

        // Assert 
        action.Should().ThrowExactly<KeyNotFoundException>();
    }

    [Fact]
    public void GetEdgesTest_KnownNode_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));
        graph.AddNode(node);

        // Act

        var action = () => graph.GetEdges(node);

        // Assert 
        action.Should().NotThrow();
    }

    [Fact]
    public void AddEdgeTest_KnownNodesEdge_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));
        var node2 = new PackageObjectReference("TestPAckage", new ObjectIndex(2));
        graph.AddNode(node);
        graph.AddNode(node2);

        // Act

        var action = () => graph.AddEdge(node, node2);

        // Assert 
        action.Should().NotThrow();
    }

    [Fact]
    public void AddEdgeTest_UnknownNodeEdge_AddsNewNode()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));
        var node2 = new PackageObjectReference("TestPAckage", new ObjectIndex(2));
        graph.AddNode(node);
        var nodeCount = graph.NodeCount;

        // Act
        graph.AddEdge(node, node2);

        // Assert 
        graph.NodeCount.Should().Be(nodeCount + 1);
    }

    [Fact]
    public void AddEdgeTest_FromToEqual_ThrowsArgumentException()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));
        var node2 = new PackageObjectReference("TestPAckage", new ObjectIndex(1));

        // Act
        var action = () => graph.AddEdge(node, node2);

        // Assert 
        action.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void AddEdgeTest_AllUnknownNodes_AddsNewNodes()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));
        var node2 = new PackageObjectReference("TestPAckage", new ObjectIndex(2));
        var nodeCount = graph.NodeCount;

        // Act
        graph.AddEdge(node, node2);

        // Assert 
        graph.NodeCount.Should().Be(nodeCount + 2);
    }

    [Fact]
    public void AddEdgeTest_DuplicateEdge_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));
        var node2 = new PackageObjectReference("TestPAckage", new ObjectIndex(2));
        graph.AddNode(node);
        graph.AddNode(node2);
        graph.AddEdge(node, node2);
        // Act

        var action = () => graph.AddEdge(node, node2);

        // Assert 
        action.Should().NotThrow();
    }

    [Fact]
    public void AddObjectDependencies_CoreObject_NoDependantNodes()
    {
        // Arrange

        var graph = new CrossPackageDependencyGraph(_packageCache);
        var coreObjectRef = new PackageObjectReference(_udkPackages.Core.PackageName, new ObjectIndex(ObjectIndex.FromExportIndex(0)));

        // Act

        graph.AddObjectDependencies(coreObjectRef);

        // Assert 
        graph.NodeCount.Should().Be(1);
    }

    [Fact]
    public void AddObjectDependencies_CoreDefaultObject_OneDependantNodes()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var coreDefaultObject = new PackageObjectReference(_udkPackages.Core.PackageName, new ObjectIndex(ObjectIndex.FromExportIndex(1)));

        // Act
        graph.AddObjectDependencies(coreDefaultObject);

        // Assert 
        graph.NodeCount.Should().Be(2);
    }


    [Fact]
    public void AddObjectDependencies_CoreComponent_OneDependantNodes()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var coreDefaultObject = new PackageObjectReference(_udkPackages.Core.PackageName, new ObjectIndex(ObjectIndex.FromExportIndex(2)));
        // Act

        graph.AddObjectDependencies(coreDefaultObject);

        // Assert 
        graph.NodeCount.Should().Be(2);
    }

    [Fact]
    public void AddObjectDependencies_CoreDefaultComponent_FourDependantNodes()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var coreDefaultObject = new PackageObjectReference(_udkPackages.Core.PackageName, new ObjectIndex(ObjectIndex.FromExportIndex(3)));

        // Act

        graph.AddObjectDependencies(coreDefaultObject);
        graph.AddObjectDependencies(coreDefaultObject);

        // Assert 
        graph.NodeCount.Should().Be(4);
    }

    [Fact]
    public void AddObjectDependencies_EngineIsAReturnValue_TenDependantNodes()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        // Core.Object.IsA.ReturnValue
        var isAReturnValue = new PackageObjectReference(_udkPackages.Engine.PackageName, new ObjectIndex(ObjectIndex.FromImportIndex(2)));

        // Act
        graph.AddObjectDependencies(isAReturnValue);

        // Assert 
        graph.NodeCount.Should().Be(9);
    }

    [Fact]
    public void AddObjectDependencies_PendingMatch_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var pendingMatch = new PackageObjectReference(_udkPackages.CustomGame.PackageName, new ObjectIndex(ObjectIndex.FromExportIndex(0)));

        // Act
        var action = () => graph.AddObjectDependencies(pendingMatch);

        // Assert 
        action.Should().NotThrow();

        var sorted = graph.TopologicalSort().Select(graph.GetReferenceFullName);
        _testOutputHelper.WriteLine(string.Join("\n", sorted));
    }

    [Fact]
    public void AddObjectDependencies_CustomPawn_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var customPawn = new PackageObjectReference(_udkPackages.CustomGame.PackageName, new ObjectIndex(ObjectIndex.FromExportIndex(3)));

        // Act
        var action = () => graph.AddObjectDependencies(customPawn);

        // Assert 
        action.Should().NotThrow();

        var sorted = graph.TopologicalSort().Select(graph.GetReferenceFullName);
        _testOutputHelper.WriteLine(string.Join("\n", sorted));
    }

    [Fact]
    public void AddObjectDependencies_CustomPlayerController_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var customPlayerController = new PackageObjectReference(_udkPackages.CustomGame.PackageName, new ObjectIndex(ObjectIndex.FromExportIndex(5)));

        // Act
        var action = () => graph.AddObjectDependencies(customPlayerController);

        // Assert 
        action.Should().NotThrow();

        var sorted = graph.TopologicalSort().Select(graph.GetReferenceFullName);
        _testOutputHelper.WriteLine(string.Join("\n", sorted));
    }

    [Fact]
    public void AddObjectDependencies_Sprite_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_packageCache);
        var sprite = new PackageObjectReference(_udkPackages.CustomGame.PackageName, new ObjectIndex(ObjectIndex.FromExportIndex(10)));

        // Act
        var action = () => graph.AddObjectDependencies(sprite);

        // Assert 
        action.Should().NotThrow();

        var sorted = graph.TopologicalSort().Select(graph.GetReferenceFullName);
        _testOutputHelper.WriteLine(string.Join("\n", sorted));
    }
}