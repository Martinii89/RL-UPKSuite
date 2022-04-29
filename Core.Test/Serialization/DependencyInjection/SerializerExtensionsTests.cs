using Core.Serialization.Tests.TestClasses;
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
        var testSerializer = services.GetRequiredService<IStreamSerializerFor<MySerializableTestOnlyDefault>>();
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
        var testSerializer = services.GetRequiredService<IStreamSerializerFor<MySerializableTest>>();
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
        var testSerializer = services.GetRequiredService<IStreamSerializerFor<MySerializableTest>>();
        var testSerializer2 = services.GetRequiredService<IStreamSerializerFor<MySerializableTestOnlyDefault>>();
        // Assert
        testSerializer.Should().NotBeNull();
        testSerializer.Should().BeOfType<MyTaggedTestSerializer>();

        testSerializer2.Should().NotBeNull();
        testSerializer2.Should().BeOfType<MySerializeTestOnlyDefaultSerializer>();
    }
}