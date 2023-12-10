namespace Core.Serialization.Tests.TestClasses;

internal class MySerializeTestOnlyDefaultSerializer : IStreamSerializer<MySerializableTestOnlyDefault>
{
    public MySerializableTestOnlyDefault Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Serialize(Stream stream, MySerializableTestOnlyDefault value)
    {
        throw new NotImplementedException();
    }
}

public class MyTestSerializer : IStreamSerializer<MySerializableTest>
{
    public MySerializableTest Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Serialize(Stream stream, MySerializableTest value)
    {
        throw new NotImplementedException();
    }
}

[FileVersion("MyTag")]
public class MyTaggedTestSerializer : IStreamSerializer<MySerializableTest>
{
    public MySerializableTest Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Serialize(Stream stream, MySerializableTest value)
    {
        throw new NotImplementedException();
    }
}

[FileVersion("InheritGroup")]
public class MyInheritTestSerializer : MyTestSerializer, IStreamSerializer<InheritTest>
{
    public new InheritTest Deserialize(Stream stream)
    {
        var baseValue = base.Deserialize(stream);
        throw new NotImplementedException();
    }

    public void Serialize(Stream stream, InheritTest value)
    {
        base.Serialize(stream, value);
        throw new NotImplementedException();
    }
}

[FileVersion("MultipleInheritance")]
public class SerializersWithMultipleInterfaces : IStreamSerializer<MySerializableTest>,
    IStreamSerializer<MySerializableTest2>
{
    MySerializableTest IStreamSerializer<MySerializableTest>.Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }

    public void Serialize(Stream stream, MySerializableTest value)
    {
        throw new NotImplementedException();
    }

    public void Serialize(Stream stream, MySerializableTest2 value)
    {
        throw new NotImplementedException();
    }

    MySerializableTest2 IStreamSerializer<MySerializableTest2>.Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }
}