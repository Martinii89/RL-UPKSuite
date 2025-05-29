using RlUpk.Core.Serialization.Abstraction;
using RlUpk.Core.Serialization.Extensions;
using RlUpk.Core.Types.PackageTables;

namespace RlUpk.Core.Serialization.Default;

/// <summary>
///     Serializers for the items in the NameTable
/// </summary>
public class NameTableItemSerializer : IStreamSerializer<NameTableItem>
{
    /// <inheritdoc />
    public NameTableItem Deserialize(Stream stream)
    {
        return new NameTableItem(stream.ReadFString(), stream.ReadUInt64());
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, NameTableItem value)
    {
        stream.WriteFString(value.Name);
        stream.WriteUInt64(value.Flags);
    }
}