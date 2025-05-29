using FluentAssertions;

using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Serialization.RocketLeague;
using RlUpk.Core.Types;
using RlUpk.Core.Types.PackageTables;
using RlUpk.TestUtils.TestUtilities;

using Xunit;

namespace RlUpk.Core.Test.Types.PackageTables;

public class ExportTableTests
{
    private const int ExportCount = 2;

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

    private readonly List<ExportTableItem> _exportTableItems = new();
    private readonly IStreamSerializer<ExportTableItem> _serializer;

    public ExportTableTests()
    {
        _exportTableItems.Add(new ExportTableItem(new ObjectIndex(-2),
            new ObjectIndex(0),
            new ObjectIndex(0),
            new FName(11),
            new ObjectIndex(0),
            0x7000000000000,
            44,
            951,
            0,
            new List<int>(),
            new FGuid(),
            0));

        _exportTableItems.Add(new ExportTableItem(new ObjectIndex(-1),
            new ObjectIndex(0),
            new ObjectIndex(0),
            new FName(9),
            new ObjectIndex(0),
            0x7000400000000,
            12,
            995,
            1,
            new List<int>(),
            new FGuid(),
            0x20000005));

        _serializer = SerializerHelper.GetSerializerFor<ExportTableItem>(typeof(ExportTableItem), RocketLeagueBase.FileVersion);
    }

    [Fact]
    public void ExportTableTest_DeserializeFromBytes_RightExports()
    {
        // Arrange
        var stream = new MemoryStream(_exportData);
        // Act
        var exportTable = new ExportTable(_serializer, stream, ExportCount);
        // Assert 
        exportTable.Should().BeEquivalentTo(_exportTableItems);
    }

    [Fact]
    public void ExportTableTest_Serialize_RightBytes()
    {
        // Arrange
        var stream = new MemoryStream();
        // Act
        var exportTable = new ExportTable();
        exportTable.AddRange(_exportTableItems);
        _serializer.WriteTArray(stream, exportTable.ToArray(), StreamSerializerForExtension.ArraySizeSerialization.NoSize);
        var streamBuffer = new ArraySegment<byte>(stream.GetBuffer(), 0, (int) stream.Length);
        // Assert 
        streamBuffer.Should().BeEquivalentTo(_exportData);
    }
}