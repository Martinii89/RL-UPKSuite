using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Serialization.Extensions;
using FluentAssertions;
using Syroot.BinaryData;
using Xunit;

namespace Core.UnrealStream.Tests;

public class StreamExtensionsTests
{
    /// <summary>
    ///     Just needs to work for testing.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private byte[] GetBytesFromStringWithZeroTermination(string value)
    {
        var newValue = value + char.MinValue;
        return Encoding.ASCII.GetBytes(newValue);
    }

    /// <summary>
    ///     Just needs to work for testing
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private string GetStringFromZeroTerminatedBytes(byte[] value)
    {
        return Encoding.ASCII.GetString(new ArraySegment<byte>(value, 0, value.Length - 1));
    }

    [Fact]
    private void CreateZeroTerminatedStringBytes()
    {
        // Arrange
        var s = "test";
        // Act
        var sBytes = GetBytesFromStringWithZeroTermination(s);
        // Assert 
        sBytes.Last().Should().Be(0);
        sBytes.SkipLast(1).ToArray().Should().BeEquivalentTo(Encoding.ASCII.GetBytes(s));
    }

    [Fact]
    private void GetZeroTerminatedStringFromBytes()
    {
        // Arrange
        var s = GetBytesFromStringWithZeroTermination("test");
        // Act
        var myString = GetStringFromZeroTerminatedBytes(s);
        // Assert 
        myString.Should().NotBeNull();
        myString.Should().Be("test");
    }

    [Fact]
    public void ReadFStringTest()
    {
        // Arrange
        var testStrings = new List<string> { "some", "text", "for", "testing" };
        var stringStream = new MemoryStream();
        foreach (var s in testStrings)
        {
            // FString.length
            stringStream.WriteInt32(s.Length + 1);
            // FString.Data
            stringStream.Write(GetBytesFromStringWithZeroTermination(s));
        }

        stringStream.Position = 0;
        // Act

        var readFsttrings = new List<string>();
        for (var i = 0; i < testStrings.Count; i++)
        {
            readFsttrings.Add(stringStream.ReadFString());
        }

        // Assert 
        readFsttrings.Should().BeEquivalentTo(testStrings);
    }

    [Fact]
    public void WriteFStringTest()
    {
        // Arrange
        var testStrings = new List<string> { "some", "text", "for", "testing" };
        var expected = new MemoryStream();

        foreach (var testString in testStrings)
        {
            expected.WriteInt32(testString.Length + 1);
            expected.Write(GetBytesFromStringWithZeroTermination(testString));
        }

        expected.Position = 0;

        // Act
        var stringStream = new MemoryStream();
        foreach (var s in testStrings)
        {
            // FString.length
            stringStream.WriteFString(s);
        }

        stringStream.Position = 0;

        // Assert 
        stringStream.GetBuffer().Should().BeEquivalentTo(expected.GetBuffer());
    }
}