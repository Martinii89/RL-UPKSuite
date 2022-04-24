using Xunit;
using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;

namespace Core.Types.Tests
{
    public class FNameTests
    {
        private readonly byte[] _nameBytes =
        {
            0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        private const int NameCount = 3;

        private readonly List<FName> _expected = new()
        {
            new FName(2, 0), new FName(1, 0), new FName(12, 0)
        };

        [Fact()]
        public void DeserializeTest()
        {
            // Arrange
            var names = new List<FName>(3);
            var stream = new MemoryStream(_nameBytes);
            // Act

            for (int i = 0; i < NameCount; i++)
            {
                names.Add(new FName(stream));
            }

            // Assert 
            names.Count.Should().Be(3);
            names.Should().BeEquivalentTo(_expected);
        }

        [Fact()]
        public void SerializeTest()
        {
            // Arrange
            var stream = new MemoryStream();

            // Act
            foreach (var fName in _expected)
            {
                fName.Serialize(stream);
            }

            var resultStream = new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length);

            // Assert 
            resultStream.Should().BeEquivalentTo(_nameBytes);
        }
    }
}