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

public class CrossPackageDependencyGraphTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public CrossPackageDependencyGraphTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void AddNodeTest_AddNode_NoThrow()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(Substitute.For<IImportResolver>());
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
        var graph = new CrossPackageDependencyGraph(Substitute.For<IImportResolver>());
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
        var graph = new CrossPackageDependencyGraph(Substitute.For<IImportResolver>());
        var node = new PackageObjectReference("TestPAckage", new ObjectIndex(1));
        graph.AddNode(node);
        var startCount = graph.NodeCount;
        // Act
        var action = () => graph.AddNode(node);

        // Assert 
        graph.NodeCount.Should().Be(startCount);
    }

    [Fact]
    public void GetEdgesTest_UnknownNode_ThrowsKeyNotFound()
    {
        // Arrange
        var graph = new CrossPackageDependencyGraph(Substitute.For<IImportResolver>());
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
        var graph = new CrossPackageDependencyGraph(Substitute.For<IImportResolver>());
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
        var graph = new CrossPackageDependencyGraph(Substitute.For<IImportResolver>());
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
        var graph = new CrossPackageDependencyGraph(Substitute.For<IImportResolver>());
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
        var graph = new CrossPackageDependencyGraph(Substitute.For<IImportResolver>());
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
        var graph = new CrossPackageDependencyGraph(Substitute.For<IImportResolver>());
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
        var graph = new CrossPackageDependencyGraph(Substitute.For<IImportResolver>());
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
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        var packageStream = File.OpenRead(@"TestData/UDK/Core.u");
        var package = UnrealPackage.DeserializeAndInitialize(packageStream, serializer, "Core");

        var packageImportResolver = Substitute.For<IImportResolver>();
        packageImportResolver.ResolveExportPackage("Core").Returns(package);

        var graph = new CrossPackageDependencyGraph(packageImportResolver);
        var coreObjectRef = new PackageObjectReference("Core", new ObjectIndex(ObjectIndex.FromExportIndex(0)));

        // Act

        graph.AddObjectDependencies(coreObjectRef);

        // Assert 
        graph.NodeCount.Should().Be(1);
    }

    [Fact]
    public void AddObjectDependencies_CoreDefaultObject_OneDependantNodes()
    {
        // Arrange
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        var packageStream = File.OpenRead(@"TestData/UDK/Core.u");
        var package = UnrealPackage.DeserializeAndInitialize(packageStream, serializer, "Core");

        var packageImportResolver = Substitute.For<IImportResolver>();
        packageImportResolver.ResolveExportPackage("Core").Returns(package);

        var graph = new CrossPackageDependencyGraph(packageImportResolver);
        var coreDefaultObject = new PackageObjectReference("Core", new ObjectIndex(ObjectIndex.FromExportIndex(1)));

        // Act

        graph.AddObjectDependencies(coreDefaultObject);

        // Assert 
        graph.NodeCount.Should().Be(2);
    }


    [Fact]
    public void AddObjectDependencies_CoreComponent_OneDependantNodes()
    {
        // Arrange
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        var packageStream = File.OpenRead(@"TestData/UDK/Core.u");
        var package = UnrealPackage.DeserializeAndInitialize(packageStream, serializer, "Core");

        var packageImportResolver = Substitute.For<IImportResolver>();
        packageImportResolver.ResolveExportPackage("Core").Returns(package);

        var graph = new CrossPackageDependencyGraph(packageImportResolver);
        var coreDefaultObject = new PackageObjectReference("Core", new ObjectIndex(ObjectIndex.FromExportIndex(2)));

        // Act

        graph.AddObjectDependencies(coreDefaultObject);

        // Assert 
        graph.NodeCount.Should().Be(2);
    }

    [Fact]
    public void AddObjectDependencies_CoreDefaultComponent_FourDependantNodes()
    {
        // Arrange
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));
        var packageStream = File.OpenRead(@"TestData/UDK/Core.u");
        var package = UnrealPackage.DeserializeAndInitialize(packageStream, serializer, "Core");

        var packageImportResolver = Substitute.For<IImportResolver>();
        packageImportResolver.ResolveExportPackage("Core").Returns(package);

        var graph = new CrossPackageDependencyGraph(packageImportResolver);
        var coreDefaultObject = new PackageObjectReference("Core", new ObjectIndex(ObjectIndex.FromExportIndex(3)));

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
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));

        var corePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Core.u"), serializer, "Core");
        var packageImportResolver = Substitute.For<IImportResolver>();
        packageImportResolver.ResolveExportPackage("Core").Returns(corePackage);
        var enginePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Engine.u"), serializer, "Engine", packageImportResolver);
        packageImportResolver.ResolveExportPackage("Engine").Returns(enginePackage);

        var importFullName = enginePackage.GetFullName(enginePackage.ImportTable[2]);


        var graph = new CrossPackageDependencyGraph(packageImportResolver);
        // Core.Object.IsA.ReturnValue
        var IsAReturnValue = new PackageObjectReference("Engine", new ObjectIndex(ObjectIndex.FromImportIndex(2)));

        // Act

        graph.AddObjectDependencies(IsAReturnValue);

        // Assert 
        graph.NodeCount.Should().Be(10);
    }

    [Fact]
    public void AddObjectDependencies_PendingMatch_NoThrow()
    {
        // Arrange
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));

        var corePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Core.u"), serializer, "Core");
        var packageImportResolver = Substitute.For<IImportResolver>();
        packageImportResolver.ResolveExportPackage("Core").Returns(corePackage);
        var enginePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Engine.u"), serializer, "Engine", packageImportResolver);
        packageImportResolver.ResolveExportPackage("Engine").Returns(enginePackage);
        var customGamePackage =
            UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/CustomGame.u"), serializer, "CustomGame", packageImportResolver);
        packageImportResolver.ResolveExportPackage("CustomGame").Returns(customGamePackage);

        var graph = new CrossPackageDependencyGraph(packageImportResolver);

        // Core.Object.IsA.ReturnValue
        var pendingMatch = new PackageObjectReference("CustomGame", new ObjectIndex(ObjectIndex.FromExportIndex(0)));

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
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));

        var corePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Core.u"), serializer, "Core");
        var packageImportResolver = Substitute.For<IImportResolver>();
        packageImportResolver.ResolveExportPackage("Core").Returns(corePackage);
        var enginePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Engine.u"), serializer, "Engine", packageImportResolver);
        packageImportResolver.ResolveExportPackage("Engine").Returns(enginePackage);
        var gameFrameworkPackage =
            UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/GameFramework.u"), serializer, "GameFramework", packageImportResolver);
        packageImportResolver.ResolveExportPackage("GameFramework").Returns(gameFrameworkPackage);
        var customGamePackage =
            UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/CustomGame.u"), serializer, "CustomGame", packageImportResolver);
        packageImportResolver.ResolveExportPackage("CustomGame").Returns(customGamePackage);

        var graph = new CrossPackageDependencyGraph(packageImportResolver);

        // Core.Object.IsA.ReturnValue
        var pendingMatch = new PackageObjectReference("CustomGame", new ObjectIndex(ObjectIndex.FromExportIndex(3)));

        // Act

        var action = () => graph.AddObjectDependencies(pendingMatch);

        // Assert 
        action.Should().NotThrow();

        var sorted = graph.TopologicalSort().Select(graph.GetReferenceFullName);
        _testOutputHelper.WriteLine(string.Join("\n", sorted));
    }

    [Fact]
    public void AddObjectDependencies_CustomPlayerController_NoThrow()
    {
        // Arrange
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));

        var corePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Core.u"), serializer, "Core");
        var packageImportResolver = Substitute.For<IImportResolver>();
        packageImportResolver.ResolveExportPackage("Core").Returns(corePackage);
        var enginePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Engine.u"), serializer, "Engine", packageImportResolver);
        packageImportResolver.ResolveExportPackage("Engine").Returns(enginePackage);
        var gameFrameworkPackage =
            UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/GameFramework.u"), serializer, "GameFramework", packageImportResolver);
        packageImportResolver.ResolveExportPackage("GameFramework").Returns(gameFrameworkPackage);
        var customGamePackage =
            UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/CustomGame.u"), serializer, "CustomGame", packageImportResolver);
        packageImportResolver.ResolveExportPackage("CustomGame").Returns(customGamePackage);

        var graph = new CrossPackageDependencyGraph(packageImportResolver);

        // Core.Object.IsA.ReturnValue
        var pendingMatch = new PackageObjectReference("CustomGame", new ObjectIndex(ObjectIndex.FromExportIndex(5)));

        // Act

        var action = () => graph.AddObjectDependencies(pendingMatch);

        // Assert 
        action.Should().NotThrow();

        var sorted = graph.TopologicalSort().Select(graph.GetReferenceFullName);
        _testOutputHelper.WriteLine(string.Join("\n", sorted));
    }

    [Fact]
    public void AddObjectDependencies_Sprite_NoThrow()
    {
        // Arrange
        var serializer = SerializerHelper.GetSerializerFor<UnrealPackage>(typeof(UnrealPackage));

        var corePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Core.u"), serializer, "Core");
        var packageImportResolver = Substitute.For<IImportResolver>();
        packageImportResolver.ResolveExportPackage("Core").Returns(corePackage);
        var enginePackage = UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/Engine.u"), serializer, "Engine", packageImportResolver);
        var gameFrameworkPackage =
            UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/GameFramework.u"), serializer, "GameFramework", packageImportResolver);
        packageImportResolver.ResolveExportPackage("GameFramework").Returns(gameFrameworkPackage);
        packageImportResolver.ResolveExportPackage("Engine").Returns(enginePackage);
        var customGamePackage =
            UnrealPackage.DeserializeAndInitialize(File.OpenRead(@"TestData/UDK/CustomGame.u"), serializer, "CustomGame", packageImportResolver);
        packageImportResolver.ResolveExportPackage("CustomGame").Returns(customGamePackage);

        var graph = new CrossPackageDependencyGraph(packageImportResolver);

        // Core.Object.IsA.ReturnValue
        var pendingMatch = new PackageObjectReference("CustomGame", new ObjectIndex(ObjectIndex.FromExportIndex(10)));

        // Act

        var action = () => graph.AddObjectDependencies(pendingMatch);

        // Assert 
        action.Should().NotThrow();

        var sorted = graph.TopologicalSort().Select(graph.GetReferenceFullName);
        _testOutputHelper.WriteLine(string.Join("\n", sorted));
    }
}