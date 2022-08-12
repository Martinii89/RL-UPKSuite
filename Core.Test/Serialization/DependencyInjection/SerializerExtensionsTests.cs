using System.Linq;
using Core.Classes;
using Core.Classes.Core;
using Core.Serialization.Abstraction;
using Core.Serialization.Default;
using Core.Serialization.Default.Object;
using Core.Serialization.RocketLeague;
using Core.Serialization.Tests.TestClasses;
using Core.Types;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Core.Serialization.DependencyInjection.Tests;

public class SerializerExtensionsTests
{
    [Fact]
    public void AddSerializersTest_CanFindATestSerializer()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        // Act
        serviceColection.UseSerializers(typeof(SerializerExtensionsTests), new SerializerOptions());
        var services = serviceColection.BuildServiceProvider();
        var testSerializer = services.GetRequiredService<IStreamSerializer<MySerializableTestOnlyDefault>>();
        // Assert
        testSerializer.Should().NotBeNull();
    }

    [Fact]
    public void AddSerializersTest_AddTAgged_CanFindTagged()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        // Act
        serviceColection.UseSerializers(typeof(SerializerExtensionsTests), new SerializerOptions("MyTag", SerializerOptions.DefaultSerializers.No));
        var services = serviceColection.BuildServiceProvider();
        var testSerializer = services.GetRequiredService<IStreamSerializer<MySerializableTest>>();
        // Assert
        testSerializer.Should().NotBeNull();
        testSerializer.Should().BeOfType<MyTaggedTestSerializer>();
    }

    [Fact]
    public void AddSerializersTest_AddTAggedAndDefault_CanFindTaggedAndDefault()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        // Act
        serviceColection.UseSerializers(typeof(SerializerExtensionsTests), new SerializerOptions("MyTag"));
        var services = serviceColection.BuildServiceProvider();
        var testSerializer = services.GetRequiredService<IStreamSerializer<MySerializableTest>>();
        var testSerializer2 = services.GetRequiredService<IStreamSerializer<MySerializableTestOnlyDefault>>();
        // Assert
        testSerializer.Should().NotBeNull();
        testSerializer.Should().BeOfType<MyTaggedTestSerializer>();

        testSerializer2.Should().NotBeNull();
        testSerializer2.Should().BeOfType<MySerializeTestOnlyDefaultSerializer>();
    }

    [Fact]
    public void AddSerializersTest_CanFindFileSummarySerializer()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        // Act
        serviceColection.UseSerializers(typeof(FileSummarySerializer),
            new SerializerOptions());
        var services = serviceColection.BuildServiceProvider();
        var testSerializer = services.GetRequiredService<IStreamSerializer<FileSummary>>();
        // Assert
        testSerializer.Should().NotBeNull();
    }

    [Fact]
    public void AddSerializersTest_CanFindObjectSerializers()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        // Act
        serviceColection.UseSerializers(typeof(DefaultObjectSerializer),
            new SerializerOptions());
        var services = serviceColection.BuildServiceProvider();
        var testSerializer1 = services.GetService<IObjectSerializer<UObject>>();
        var testSerializer2 = services.GetService<IObjectSerializer<UField>>();
        var testSerializer3 = services.GetService<IObjectSerializer<UStruct>>();
        // Assert
        testSerializer1.Should().NotBeNull();
        testSerializer2.Should().NotBeNull();
        testSerializer3.Should().NotBeNull();
    }

    [Fact]
    public void AddSerializersTest_CanFindObjectSerializerList()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        // Act
        serviceColection.UseSerializers(typeof(DefaultObjectSerializer),
            new SerializerOptions());
        var services = serviceColection.BuildServiceProvider();
        var testSerializer1 = services.GetServices<IObjectSerializer>().ToList();
        // Assert
        testSerializer1.Should().NotBeNull();
        testSerializer1.Should().HaveCountGreaterThan(2);
    }

    [Fact]
    public void AddSerializersTest_FactoryCanFindObjectSerializer()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        // Act
        serviceColection.UseSerializers(typeof(DefaultObjectSerializer),
            new SerializerOptions());
        serviceColection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
        var services = serviceColection.BuildServiceProvider();
        var factory = services.GetService<IObjectSerializerFactory>();
        var objectSerializer = factory!.GetSerializer(typeof(UObject));
        // Assert
        factory.Should().NotBeNull();
        objectSerializer.Should().NotBeNull();
        objectSerializer.Should().BeOfType<DefaultObjectSerializer>();
    }

    [Fact]
    public void AddSerializersTest_FactoryCanFindTaggedObjectSerializer()
    {
        // Arrange
        var serviceColection = new ServiceCollection();
        // Act
        serviceColection.UseSerializers(typeof(DefaultObjectSerializer),
            new SerializerOptions { FileVersion = RocketLeagueBase.FileVersion });
        serviceColection.AddSingleton<IObjectSerializerFactory, ObjectSerializerFactory>();
        var services = serviceColection.BuildServiceProvider();
        var factory = services.GetService<IObjectSerializerFactory>();
        var objectSerializer = factory!.GetSerializer(typeof(UClass));
        // Assert
        factory.Should().NotBeNull();
        objectSerializer.Should().NotBeNull();
        objectSerializer.Should().BeOfType<RLClassSerializer>();
    }
}