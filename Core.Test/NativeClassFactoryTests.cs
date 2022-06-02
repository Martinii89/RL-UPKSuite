using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Types;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Core.Tests;

public class NativeClassFactoryTests
{
    [Fact]
    public void GetNativeCoreClassesTest_ReturnsValidClasses()
    {
        // Arrange
        var sut = new NativeClassFactory();
        var corePackage = new UnrealPackage
        {
            PackageName = "Core"
        };
        var packageRoot = new UPackage(corePackage.GetOrAddName("Core"), UClass.StaticClass, null, corePackage);
        corePackage.PackageRoot = packageRoot;
        // Act

        var coreNatives = sut.GetNativeClasses(corePackage);
        // Assert 
        coreNatives.Should().HaveCount(29);
        coreNatives.Should().AllSatisfy(x =>
        {
            x.Value.Should().NotBeNull();
            x.Value.OwnerPackage.Should().Be(corePackage);
            x.Value.InstanceConstructor.Should().NotBeNull();
            x.Value.Outer.Should().NotBeNull();
            x.Value.Outer.Should().Be(corePackage.PackageRoot);
            x.Value.Outer!.Name.Should().Be("Core");
        });
    }


    [Fact]
    public void GetNativeEngineClassesTest_ReturnsValidClasses()
    {
        // Arrange
        var sut = new NativeClassFactory();
        var enginePackage = new UnrealPackage
        {
            PackageName = "Engine"
        };
        var packageRoot = new UPackage(enginePackage.GetOrAddName("Engine"), UClass.StaticClass, null, enginePackage);
        enginePackage.PackageRoot = packageRoot;
        // Act

        var engineNatives = sut.GetNativeClasses(enginePackage);
        // Assert 
        engineNatives.Should().AllSatisfy(x =>
        {
            var clz = x.Value;
            clz.Should().NotBeNull();
            clz.OwnerPackage.Should().Be(enginePackage);
            clz.InstanceConstructor.Should().NotBeNull();
            clz.Outer.Should().NotBeNull();
            clz.Outer.Should().Be(enginePackage.PackageRoot);
            clz.Outer!.Name.Should().Be("Engine");
            clz.SuperClass.Should().NotBeNull();
        });
    }

    [Fact]
    public void GetNativeEngineClassesTest_CallsObjectSerializerFactory()
    {
        // Arrange
        var nativeFactory = new NativeClassFactory();
        var enginePackage = new UnrealPackage
        {
            PackageName = "Engine"
        };
        var packageRoot = new UPackage(enginePackage.GetOrAddName("Engine"), UClass.StaticClass, null, enginePackage);
        enginePackage.PackageRoot = packageRoot;
        var sut = Substitute.For<IObjectSerializerFactory>();
        enginePackage.ObjectSerializerFactory = sut;

        // Act
        var engineNatives = nativeFactory.GetNativeClasses(enginePackage);
        // Assert 
        sut.ReceivedWithAnyArgs(3).GetSerializer(default);
    }

    [Fact]
    public void GetNativeEngineClassesTest_StaticMeshInitializedWithCustomProperties()
    {
        // Arrange
        var nativeFactory = new NativeClassFactory();
        var enginePackage = new UnrealPackage
        {
            PackageName = "Engine"
        };
        var packageRoot = new UPackage(enginePackage.GetOrAddName("Engine"), UClass.StaticClass, null, enginePackage);
        enginePackage.PackageRoot = packageRoot;
        var serializerFactory = Substitute.For<IObjectSerializerFactory>();
        enginePackage.ObjectSerializerFactory = serializerFactory;

        // Act
        var engineNatives = nativeFactory.GetNativeClasses(enginePackage);
        var sut = engineNatives["StaticMesh"];

        // Assert 
        sut.NativeProperties.Should().NotBeEmpty();
    }
}