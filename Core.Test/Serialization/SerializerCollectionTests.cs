using FluentAssertions;

using RlUpk.Core.Serialization;
using RlUpk.Core.Test.Serialization.TestClasses;

using Xunit;

namespace RlUpk.Core.Test.Serialization;

public class SerializerCollectionTests
{
    [Fact]
    public void GetSerializerForTest_WithNoTag_ReturnNonNullAndCorrectSerializer()
    {
        // Arrange
        var collection = new SerializerCollection();
        collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "");
        // Act
        var serializer = collection.GetSerializerFor<MySerializableTest>();

        // Assert 
        serializer.Should().NotBeNull();
        serializer.Should().BeOfType<MyTestSerializer>();
    }

    [Fact]
    public void GetSerializerForTest_WithTag_ReturnNonNullAndCorrectSerializer()
    {
        // Arrange
        var collection = new SerializerCollection();
        collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "MyTag");
        // Act
        var serializer = collection.GetSerializerFor<MySerializableTest>();

        // Assert 
        serializer.Should().NotBeNull();
        serializer.Should().BeOfType<MyTaggedTestSerializer>();
    }

    [Fact]
    public void AddSerializersFromAssemblyTest_WithNoTag_ShouldContainMyUntaggedTestSerializer()
    {
        // Arrange
        var collection = new SerializerCollection();

        // Act
        collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "");

        // Assert 
        collection.Serializers.Should().NotBeEmpty();
        collection.Serializers.Should().HaveCount(2);
        collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should()
            .BeOfType<MyTestSerializer>();
        collection.Serializers.Should().ContainKey(typeof(MySerializableTestOnlyDefault)).WhoseValue.Should()
            .BeOfType<MySerializeTestOnlyDefaultSerializer>();
    }

    [Fact]
    public void AddSerializersFromAssemblyTest_WithTag_ShouldContainMyTaggedTestSerializer()
    {
        // Arrange
        var collection = new SerializerCollection();

        // Act
        collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "MyTag");

        // Assert 
        collection.Serializers.Should().NotBeEmpty();
        collection.Serializers.Should().HaveCount(1);
        collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should()
            .BeOfType<MyTaggedTestSerializer>();
    }

    [Fact]
    public void AddDefaultSerializersFromAssemblyTest_FindsUntaggedSerializer()
    {
        // Arrange
        var collection = new SerializerCollection();

        // Act
        collection.AddDefaultSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly);

        // Assert 
        collection.Serializers.Should().NotBeEmpty();
        collection.Serializers.Should().HaveCount(2);
        collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should()
            .BeOfType<MyTestSerializer>();
        collection.Serializers.Should().ContainKey(typeof(MySerializableTestOnlyDefault)).WhoseValue.Should()
            .BeOfType<MySerializeTestOnlyDefaultSerializer>();
    }

    [Fact]
    public void AddMissingDefaultSerializersFromAssemblyTest_AddsDefaultSerializersForMySerializableTestOnlyDefault()
    {
        // Arrange
        var collection = new SerializerCollection();

        // Act
        collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "MyTag");
        var action = () =>
            collection.AddMissingDefaultSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly);

        // Assert 
        collection.Serializers.Should().NotBeEmpty();
        collection.Serializers.Should().HaveCount(1);
        collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should()
            .BeOfType<MyTaggedTestSerializer>();
        collection.Serializers.Should().NotContainKey(typeof(MySerializableTestOnlyDefault));

        action.Invoke();
        collection.Serializers.Should().HaveCount(2);
        collection.Serializers.Should().ContainKey(typeof(MySerializableTestOnlyDefault)).WhoseValue.Should()
            .BeOfType<MySerializeTestOnlyDefaultSerializer>();
    }

    [Fact]
    public void
        AddSerializersFromAssemblyTest_TagSetHasSerializersWithInheritance_FindsSerializersIgnoresBaseInterfaceAndNoThrow()
    {
        // Arrange
        var collection = new SerializerCollection();

        // Act
        var action = () =>
            collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "InheritGroup");

        // Assert 
        action.Should().NotThrow();
        collection.Serializers.Should().NotBeEmpty();
        collection.Serializers.Should().HaveCount(1);
        collection.Serializers.Should().NotContainKey(typeof(MySerializableTest));
        collection.Serializers.Should().ContainKey(typeof(InheritTest)).WhoseValue.Should()
            .BeOfType<MyInheritTestSerializer>();
    }

    [Fact]
    public void
        AddSerializersFromAssemblyTest_TagSetHasSerializersWithMultipleInterfacesImplemented_FindsTwoSerializersOfSameType()
    {
        // Arrange
        var collection = new SerializerCollection();

        // Act
        collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "MultipleInheritance");

        // Assert 
        collection.Serializers.Should().HaveCount(2);
        collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should()
            .BeOfType<SerializersWithMultipleInterfaces>();
        collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should()
            .BeOfType<SerializersWithMultipleInterfaces>();
    }

    [Fact]
    public void AddSerializerTest_ManuallyRegisterSerializer_IsRegistered()
    {
        // Arrange
        var collection = new SerializerCollection();

        // Act
        collection.AddSerializer(new MyTestSerializer());

        // Assert 
        collection.Serializers.Should().HaveCount(1);
        collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should()
            .BeOfType<MyTestSerializer>();
    }

    [Fact]
    public void AddSerializerTest_ManuallyRegisterSerializerTwice_Throws()
    {
        // Arrange
        var collection = new SerializerCollection();

        // Act
        collection.AddSerializer(new MyTestSerializer());
        var action = () => collection.AddSerializer(new MyTestSerializer());

        // Assert 
        //throw new InvalidOperationException($"Serializer for {serializableType} already registered");
        action.Should().ThrowExactly<InvalidOperationException>()
            .WithMessage($"Serializer for {nameof(MySerializableTest)} already registered");
    }
}