using System;
using System.IO;

namespace Core.Serialization.Tests.TestClasses;

internal class MySerializeTestOnlyDefaultSerializer : IStreamSerializerFor<MySerializableTestOnlyDefault>
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

public class MyTestSerializer : IStreamSerializerFor<MySerializableTest>
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
public class MyTaggedTestSerializer : IStreamSerializerFor<MySerializableTest>
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
public class MyInheritTestSerializer : MyTestSerializer, IStreamSerializerFor<InheritTest>
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
public class SerializersWithMultipleInterfaces : IStreamSerializerFor<MySerializableTest>,
    IStreamSerializerFor<MySerializableTest2>
{
    MySerializableTest IStreamSerializerFor<MySerializableTest>.Deserialize(Stream stream)
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

    MySerializableTest2 IStreamSerializerFor<MySerializableTest2>.Deserialize(Stream stream)
    {
        throw new NotImplementedException();
    }
}