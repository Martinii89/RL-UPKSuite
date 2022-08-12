using Core.Classes.Engine;
using Core.Classes.Engine.Structs;

namespace Core.Serialization.Default.Object.Engine.Struct;

public class ByteBulkDataSerializer : IStreamSerializer<FByteBulkData>
{
    public FByteBulkData Deserialize(Stream stream)
    {
        var data = new FByteBulkData();
        data.BulkDataFlags = (BulkDataFlags) stream.ReadUInt32();
        data.ElementCount = stream.ReadInt32();
        data.BulkDataSizeOnDisk = stream.ReadInt32();
        data.BulkDataOffsetInFile = stream.ReadInt32();
        if (data.BulkDataSizeOnDisk > 0)
        {
            data.BulkData = stream.ReadBytes(data.BulkDataSizeOnDisk);
        }

        return data;
    }

    public void Serialize(Stream stream, FByteBulkData value)
    {
        stream.Write((uint) value.BulkDataFlags);
        stream.Write(value.ElementCount);
        stream.Write(value.BulkDataSizeOnDisk);
        stream.Write((int) (stream.Position + 4));
        if (value.BulkDataSizeOnDisk > 0 && !value.StoredInSeparateFile)
        {
            stream.Write(value.BulkData, 0, value.BulkDataSizeOnDisk);
        }
    }
}