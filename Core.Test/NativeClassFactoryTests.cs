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
        engineNatives.Should().HaveCount(3);
        engineNatives.Should().AllSatisfy(x =>
        {
            x.Value.Should().NotBeNull();
            x.Value.OwnerPackage.Should().Be(enginePackage);
            x.Value.InstanceConstructor.Should().NotBeNull();
            x.Value.Outer.Should().NotBeNull();
            x.Value.Outer.Should().Be(enginePackage.PackageRoot);
            x.Value.Outer!.Name.Should().Be("Engine");
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
}