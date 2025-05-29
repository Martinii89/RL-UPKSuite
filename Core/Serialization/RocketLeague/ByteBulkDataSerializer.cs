using RlUpk.Core.Classes.Engine;
using RlUpk.Core.Classes.Engine.Structs;
using RlUpk.Core.Serialization.Abstraction;

namespace RlUpk.Core.Serialization.RocketLeague;

[FileVersion(RocketLeagueBase.FileVersion)]
public class ByteBulkDataSerializer : IStreamSerializer<FByteBulkData>
{
    public FByteBulkData Deserialize(Stream stream)
    {
        var data = new FByteBulkData();
        data.BulkDataFlags = (BulkDataFlags) stream.ReadUInt32();
        data.ElementCount = stream.ReadInt32();
        data.BulkDataSizeOnDisk = stream.ReadInt32();
        if (data.StoredInSeparateFile)
        {
            data.BulkDataOffsetInFile = (int) stream.ReadInt64();
        }
        else
        {
            data.BulkDataOffsetInFile = (int) stream.Position;
        }

        if (data.BulkDataSizeOnDisk > 0 && !data.StoredInSeparateFile)
        {
            data.BulkData = stream.ReadBytes(data.BulkDataSizeOnDisk);
        }

        return data;
    }

    public void Serialize(Stream stream, FByteBulkData value)
    {
        throw new NotImplementedException();
    }
}