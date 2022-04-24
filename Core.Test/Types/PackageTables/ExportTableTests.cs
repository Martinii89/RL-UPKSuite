using Xunit;
using Core.Types.PackageTables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Core.Types.PackageTables.Tests
{
    public class ExportTableTests
    {
        private readonly byte[] _exportData =
        {
            0xFE, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0B, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x07, 0x00,
            0x2C, 0x00, 0x00, 0x00, 0xB7, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x09, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x07, 0x00, 0x0C, 0x00, 0x00, 0x00, 0xE3, 0x03, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x20
        };

        private const int ExportCount = 2;

        private readonly List<ExportTableItem> ExportTableItems = new List<ExportTableItem>();

        public ExportTableTests()
        {
            ExportTableItems.Add(new ExportTableItem(new ObjectIndex(-2),
                new ObjectIndex(0),
                new ObjectIndex(0),
                new FName(11),
                new ObjectIndex(0),
                0x7000000000000,
                44,
                951,
                0,
                new TArray<int>(),
                new FGuid(),
                0));

            ExportTableItems.Add(new ExportTableItem(new ObjectIndex(-1),
                new ObjectIndex(0),
                new ObjectIndex(0),
                new FName(9),
                new ObjectIndex(0),
                0x7000400000000,
                12,
                995,
                1,
                new TArray<int>(),
                new FGuid(),
                0x20000005));
        }

        [Fact()]
        public void ExportTableTest_DeserializeFromBytes_RightExports()
        {
            // Arrange
            var stream = new MemoryStream(_exportData);
            // Act
            var importTable = new ExportTable(stream, 0, ExportCount);
            // Assert 
            importTable.Exports.Should().BeEquivalentTo(ExportTableItems);
        }

        [Fact()]
        public void ExportTableTest_Serialize_RightBytes()
        {
            // Arrange
            var stream = new MemoryStream();
            // Act
            var importTable = new ExportTable(ExportTableItems);
            importTable.Serialize(stream);
            var streamBuffer = new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length);
            // Assert 
            streamBuffer.Should().BeEquivalentTo(_exportData);
        }
    }
}