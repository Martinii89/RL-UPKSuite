using System;
using System.Collections.Generic;
using System.IO;
using Core.Serialization;
using Core.Test.TestUtilities;
using FluentAssertions;
using Xunit;

namespace Core.Types.Tests;

public class FNameTests
{
    private const int NameCount = 3;

    private readonly List<FName> _expected = new()
    {
        new FName(2), new FName(1), new FName(12)
    };

    private readonly byte[] _nameBytes =
    {
        0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
    };

    private readonly IStreamSerializerFor<FName> _serializer;

    public FNameTests()
    {
        _serializer = SerializerHelper.GetSerializerFor<FName>(typeof(FName));
    }

    [Fact]
    public void DeserializeTest()
    {
        // Arrange
        var stream = new MemoryStream(_nameBytes);
        // Act

        var names = _serializer.ReadTArrayToList(stream, NameCount);

        // Assert 
        names.Count.Should().Be(3);
        names.Should().BeEquivalentTo(_expected);
    }

    [Fact]
    public void SerializeTest()
    {
        // Arrange
        var stream = new MemoryStream();

        // Act
        _serializer.WriteTArray(stream, _expected.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);

        var resultStream = new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length);

        // Assert 
        resultStream.Should().BeEquivalentTo(_nameBytes);
    }
}