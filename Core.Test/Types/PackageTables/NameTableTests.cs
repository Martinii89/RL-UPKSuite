﻿using Core.Serialization;
using Core.Test.TestUtilities;
using FluentAssertions;
using Xunit;

namespace Core.Types.PackageTables.Tests;

public class NameTableTests
{
    private const int NamesCountInTestData = 13;

    private readonly List<string> _names = new()
    {
        "ArrayProperty", "Class", "Core", "Engine", "None", "ObjectReferencer", "Package", "ProductThumbnailAsset_TA", "ReferencedObjects",
        "RocketPass_Premium_T", "RocketPass_Premium_T_SF", "StandaloneSeekFreeReferencer", "TAGame"
    };

    private readonly byte[] _nameTableBytes =
    {
        0x0E, 0x00, 0x00, 0x00, 0x41, 0x72, 0x72, 0x61, 0x79, 0x50, 0x72, 0x6F, 0x70, 0x65, 0x72, 0x74,
        0x79, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x07, 0x00, 0x06, 0x00, 0x00, 0x00, 0x43, 0x6C,
        0x61, 0x73, 0x73, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x07, 0x00, 0x05, 0x00, 0x00, 0x00,
        0x43, 0x6F, 0x72, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x07, 0x00, 0x07, 0x00, 0x00,
        0x00, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x07, 0x00,
        0x05, 0x00, 0x00, 0x00, 0x4E, 0x6F, 0x6E, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x07,
        0x00, 0x11, 0x00, 0x00, 0x00, 0x4F, 0x62, 0x6A, 0x65, 0x63, 0x74, 0x52, 0x65, 0x66, 0x65, 0x72,
        0x65, 0x6E, 0x63, 0x65, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x07, 0x00, 0x08, 0x00,
        0x00, 0x00, 0x50, 0x61, 0x63, 0x6B, 0x61, 0x67, 0x65, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00,
        0x07, 0x00, 0x19, 0x00, 0x00, 0x00, 0x50, 0x72, 0x6F, 0x64, 0x75, 0x63, 0x74, 0x54, 0x68, 0x75,
        0x6D, 0x62, 0x6E, 0x61, 0x69, 0x6C, 0x41, 0x73, 0x73, 0x65, 0x74, 0x5F, 0x54, 0x41, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x10, 0x00, 0x07, 0x00, 0x12, 0x00, 0x00, 0x00, 0x52, 0x65, 0x66, 0x65, 0x72,
        0x65, 0x6E, 0x63, 0x65, 0x64, 0x4F, 0x62, 0x6A, 0x65, 0x63, 0x74, 0x73, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x10, 0x00, 0x07, 0x00, 0x15, 0x00, 0x00, 0x00, 0x52, 0x6F, 0x63, 0x6B, 0x65, 0x74, 0x50,
        0x61, 0x73, 0x73, 0x5F, 0x50, 0x72, 0x65, 0x6D, 0x69, 0x75, 0x6D, 0x5F, 0x54, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x10, 0x00, 0x07, 0x00, 0x18, 0x00, 0x00, 0x00, 0x52, 0x6F, 0x63, 0x6B, 0x65, 0x74,
        0x50, 0x61, 0x73, 0x73, 0x5F, 0x50, 0x72, 0x65, 0x6D, 0x69, 0x75, 0x6D, 0x5F, 0x54, 0x5F, 0x53,
        0x46, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x07, 0x00, 0x1D, 0x00, 0x00, 0x00, 0x53, 0x74,
        0x61, 0x6E, 0x64, 0x61, 0x6C, 0x6F, 0x6E, 0x65, 0x53, 0x65, 0x65, 0x6B, 0x46, 0x72, 0x65, 0x65,
        0x52, 0x65, 0x66, 0x65, 0x72, 0x65, 0x6E, 0x63, 0x65, 0x72, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10,
        0x00, 0x07, 0x00, 0x07, 0x00, 0x00, 0x00, 0x54, 0x41, 0x47, 0x61, 0x6D, 0x65, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x10, 0x00, 0x07, 0x00
    };

    private readonly IStreamSerializer<NameTableItem> _serializer;

    public NameTableTests()
    {
        _serializer = SerializerHelper.GetSerializerFor<NameTableItem>(typeof(NameTableItem));
    }

    [Fact]
    public void NameTableTest_DeserializeNameTable_RightAmountOfNames()
    {
        // Arrange
        var tableStream = new MemoryStream(_nameTableBytes);

        // Act

        NameTable nameTable = new(_serializer, tableStream, NamesCountInTestData);
        // Assert 

        nameTable.Count.Should().Be(NamesCountInTestData);
    }

    [Fact]
    public void NameTableTest_DeserializeNameTable_RightNames()
    {
        // Arrange
        var tableStream = new MemoryStream(_nameTableBytes);

        // Act
        var nameTable = new NameTable(_serializer, tableStream, NamesCountInTestData);

        // Assert 
        nameTable.Select(x => x.Name).Should().BeEquivalentTo(_names);
    }

    [Fact]
    public void NameTableTest_SerializeAndDeserializeNameTable_EqualBytes()
    {
        // Arrange
        var tableStream = new MemoryStream(_nameTableBytes);
        var serializedStream = new MemoryStream();
        var nameTable = new NameTable(_serializer, tableStream, NamesCountInTestData);

        // Act
        _serializer.WriteTArray(serializedStream, nameTable.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);
        serializedStream.Position = 0;

        // Assert 
        var outBuffer = new ArraySegment<byte>(serializedStream.GetBuffer(), 0, (int) serializedStream.Length);
        serializedStream.Length.Should().Be(_nameTableBytes.Length);
        outBuffer.Should().BeEquivalentTo(_nameTableBytes);
    }
}