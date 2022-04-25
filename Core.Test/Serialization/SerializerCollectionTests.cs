using System.IO;
using FluentAssertions;
using Xunit;

namespace Core.Serialization.Tests
{
    public class SerializerCollectionTests
    {
        [Fact()]
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

        [Fact()]
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

        [Fact()]
        public void AddSerializersFromAssemblyTest_WithNoTag_ShouldContainMyUntaggedTestSerializer()
        {
            // Arrange
            var collection = new SerializerCollection();

            // Act
            collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "");

            // Assert 
            collection.Serializers.Should().NotBeEmpty();
            collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should().BeOfType<MyTestSerializer>();
            collection.Serializers.Should().ContainKey(typeof(MySerializableTestOnlyDefault)).WhoseValue.Should().BeOfType<MySerializeTestOnlyDefaultSerializer>();
        }

        [Fact()]
        public void AddSerializersFromAssemblyTest_WithTag_ShouldContainMyTaggedTestSerializer()
        {
            // Arrange
            var collection = new SerializerCollection();

            // Act
            collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "MyTag");

            // Assert 
            collection.Serializers.Should().NotBeEmpty();
            collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should().BeOfType<MyTaggedTestSerializer>();
            collection.Serializers.Should().NotContainKey(typeof(MySerializableTestOnlyDefault));
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
            collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should().BeOfType<MyTestSerializer>();
            collection.Serializers.Should().ContainKey(typeof(MySerializableTestOnlyDefault)).WhoseValue.Should().BeOfType<MySerializeTestOnlyDefaultSerializer>();
        }

        [Fact]
        public void AddMissingDefaultSerializersFromAssemblyTest_AddsDefaultSerializersForMySerializableTestOnlyDefault()
        {
            // Arrange
            var collection = new SerializerCollection();

            // Act
            collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "MyTag");
            collection.AddMissingDefaultSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly);

            // Assert 
            collection.Serializers.Should().NotBeEmpty();
            collection.Serializers.Should().ContainKey(typeof(MySerializableTest)).WhoseValue.Should().BeOfType<MyTaggedTestSerializer>();
            collection.Serializers.Should().ContainKey(typeof(MySerializableTestOnlyDefault)).WhoseValue.Should().BeOfType<MySerializeTestOnlyDefaultSerializer>();
        }

        [Fact(Skip = "Inhertiance in serializers not supported")]
        public void AddDefaultSerializersFromAssemblyTest_FindsInheritedUntaggedSerializer()
        {
            // Arrange
            var collection = new SerializerCollection();

            // Act
            collection.AddSerializersFromAssembly(typeof(SerializerCollectionTests).Assembly, "InheritGroup");

            // Assert 
            collection.Serializers.Should().NotBeEmpty();
            collection.Serializers.Should().ContainKey(typeof(InheritTest)).WhoseValue.Should().BeOfType<MyInheritTestSerializer>();
        }
    }

    public class MySerializableTest
    {
        private int x;
    }

    public class MySerializableTestOnlyDefault
    {
        private int x;
    }

    public class InheritTest : MySerializableTest
    {

    }

    class MySerializeTestOnlyDefaultSerializer : IStreamSerializerFor<MySerializableTestOnlyDefault>
    {
        public MySerializableTestOnlyDefault Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(Stream stream, ref MySerializableTestOnlyDefault value)
        {
            throw new System.NotImplementedException();
        }
    }


    public class MyTestSerializer : IStreamSerializerFor<MySerializableTest>
    {
        public MySerializableTest Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(Stream stream, ref MySerializableTest value)
        {
            throw new System.NotImplementedException();
        }
    }

    [FileVersion("MyTag")]
    public class MyTaggedTestSerializer : IStreamSerializerFor<MySerializableTest>
    {
        public MySerializableTest Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(Stream stream, ref MySerializableTest value)
        {
            throw new System.NotImplementedException();
        }
    }

    [FileVersion("InheritGroup")]
    public class MyInheritTestSerializer : MyTestSerializer, IStreamSerializerFor<InheritTest>
    {
        public new InheritTest Deserialize(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(Stream stream, ref InheritTest value)
        {
            //base.Serialize(stream, ref value);
            throw new System.NotImplementedException();
        }
    }
}