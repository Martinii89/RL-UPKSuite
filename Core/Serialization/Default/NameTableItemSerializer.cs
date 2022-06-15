using Core.Serialization.Extensions;
using Core.Types.PackageTables;

namespace Core.Serialization.Default;

/// <summary>
///     Serializers for the items in the NameTable
/// </summary>
public class NameTableItemSerializer : IStreamSerializer<NameTableItem>
{
    /// <inheritdoc />
    public NameTableItem Deserialize(Stream stream)
    {
        return new NameTableItem(stream.ReadFString(), stream.ReadUInt64());
        //return new NameTableItem
        //{
        //    Name = stream.ReadFString(),
        //    Flags = stream.ReadUInt64()
        //};
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, NameTableItem value)
    {
        stream.WriteFString(value.Name);
        stream.WriteUInt64(value.Flags);
    }
}