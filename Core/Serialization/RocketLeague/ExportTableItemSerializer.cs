using Core.Types;
using Core.Types.PackageTables;

namespace Core.Serialization.RocketLeague;

/// <summary>
///     Serializer for the items in the export table in rocket league packages. Difference is that SerialOffset is a 64bit
///     int.
/// </summary>
public class ExportTableItemSerializer : RocketLeagueBase, IStreamSerializer<ExportTableItem>
{
    private readonly IStreamSerializer<FGuid> _guidSerializer;
    private readonly IStreamSerializer<int> _intSerializer;
    private readonly IStreamSerializer<FName> _nameSerializer;
    private readonly IStreamSerializer<ObjectIndex> _objectIndexSerializer;


    /// <summary>
    ///     The serializers requires a name and object reference serializer
    /// </summary>
    /// <param name="nameSerializer"></param>
    /// <param name="objectIndexSerializer"></param>
    /// <param name="intSerializer"></param>
    /// <param name="guidSerializer"></param>
    public ExportTableItemSerializer(IStreamSerializer<FName> nameSerializer, IStreamSerializer<ObjectIndex> objectIndexSerializer,
        IStreamSerializer<int> intSerializer, IStreamSerializer<FGuid> guidSerializer)
    {
        _nameSerializer = nameSerializer;
        _objectIndexSerializer = objectIndexSerializer;
        _intSerializer = intSerializer;
        _guidSerializer = guidSerializer;
    }

    /// <inheritdoc />
    public ExportTableItem Deserialize(Stream stream)
    {
        var item = new ExportTableItem();
        item.ClassIndex = _objectIndexSerializer.Deserialize(stream);
        item.SuperIndex = _objectIndexSerializer.Deserialize(stream);
        item.OuterIndex = _objectIndexSerializer.Deserialize(stream);
        item.ObjectName = _nameSerializer.Deserialize(stream);
        item.ArchetypeIndex = _objectIndexSerializer.Deserialize(stream);
        item.ObjectFlags = stream.ReadUInt64();
        item.SerialSize = stream.ReadInt32();
        item.SerialOffset = stream.ReadInt64();
        item.ExportFlags = stream.ReadInt32();
        item.NetObjects = _intSerializer.ReadTArrayToList(stream);
        item.PackageGuid = _guidSerializer.Deserialize(stream);
        item.PackageFlags = stream.ReadInt32();
        return item;
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, ExportTableItem value)
    {
        _objectIndexSerializer.Serialize(stream, value.ClassIndex);
        _objectIndexSerializer.Serialize(stream, value.SuperIndex);
        _objectIndexSerializer.Serialize(stream, value.OuterIndex);
        _nameSerializer.Serialize(stream, value.ObjectName);
        _objectIndexSerializer.Serialize(stream, value.ArchetypeIndex);
        stream.WriteUInt64(value.ObjectFlags);
        stream.WriteInt32(value.SerialSize);
        stream.WriteInt64(value.SerialOffset);
        stream.WriteInt32(value.ExportFlags);
        _intSerializer.WriteTArray(stream, value.NetObjects.ToArray());
        _guidSerializer.Serialize(stream, value.PackageGuid);
        stream.WriteInt32(value.PackageFlags);
    }
}