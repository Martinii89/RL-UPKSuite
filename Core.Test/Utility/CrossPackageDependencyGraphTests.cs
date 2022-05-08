using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Test.TestUtilities;
using Core.Types;
using Core.Types.PackageTables;
using FluentAssertions;
using NSubstitute;
using Xunit;
using Xunit.Abstractions;

namespace Core.Utility.Tests;

public class UdkPackages
{
    public readonly UnrealPackage Core;
    public readonly UnrealPackage CustomGame;
    public readonly UnrealPackage Engine;
    public readonly UnrealPackage GameFramework;
    public readonly IImportResolver ImportResolver;

    public UdkPackages()
    {
        var core = new MemoryStream(File.ReadAllBytes(@"TestData/UDK/Core.u"));
        var engine = new MemoryStream(File.ReadAllBytes(@"TestData/UDK/Engine.u"));
        var customGame = new MemoryStream(File.ReadAllBytes(@"TestData/UDK/CustomGame.u"));
        var gameFramework = new MemoryStream(File.ReadAllBytes(@"TestData/UDK/GameFramework.u"));

        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        ImportResolver = Substitute.For<IImportResolver>();

        Core = UnrealPackage.DeserializeAndInitialize(core, serializer, "Core");
        ImportResolver.ResolveExportPackage("Core").Returns(Core);

        Engine = UnrealPackage.DeserializeAndInitialize(engine, serializer, "Engine", ImportResolver);
        ImportResolver.ResolveExportPackage("Engine").Returns(Engine);

        GameFramework = UnrealPackage.DeserializeAndInitialize(customGame, serializer, "GameFramework", ImportResolver);
        ImportResolver.ResolveExportPackage("GameFramework").Returns(GameFramework);

        CustomGame = UnrealPackage.DeserializeAndInitialize(gameFramework, serializer, "CustomGame", ImportResolver);
        ImportResolver.ResolveExportPackage("CustomGame").Returns(CustomGame);
    }
}

public class CrossPackageDependencyGraphTests : IClassFixture<UdkPackages>
{
    private readonly IImportResolver _importResolver;
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly UdkPackages _udkPackages;

    public CrossPackageDependencyGraphTests(ITestOutputHelper testOutputHelper, UdkPackages udkPackages)
    {
        _testOutputHelper = testOutputHelper;
        _udkPackages = udkPackages;
        _importResolver = udkPackages.ImportResolver;
    }

    [Fact]
    public void AddNodeTest_AddNode_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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

        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
        // Core.Object.IsA.ReturnValue
        var isAReturnValue = new PackageObjectReference(_udkPackages.Engine.PackageName, new ObjectIndex(ObjectIndex.FromImportIndex(2)));

        // Act
        graph.AddObjectDependencies(isAReturnValue);

        // Assert 
        graph.NodeCount.Should().Be(10);
    }

    [Fact]
    public void AddObjectDependencies_PendingMatch_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
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
        var graph = new CrossPackageDependencyGraph(_importResolver);
        var sprite = new PackageObjectReference(_udkPackages.CustomGame.PackageName, new ObjectIndex(ObjectIndex.FromExportIndex(10)));

        // Act
        var action = () => graph.AddObjectDependencies(sprite);

        // Assert 
        action.Should().NotThrow();

        var sorted = graph.TopologicalSort().Select(graph.GetReferenceFullName);
        _testOutputHelper.WriteLine(string.Join("\n", sorted));
    }
}