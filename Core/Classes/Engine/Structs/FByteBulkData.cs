namespace Core.Classes.Engine.Structs;

public class FByteBulkData
{
    public BulkDataFlags BulkDataFlags { get; set; }
    public int ElementCount { get; set; }
    public int BulkDataSizeOnDisk { get; set; }
    public int BulkDataOffsetInFile { get; set; }
    public byte[] BulkData { get; set; } = Array.Empty<byte>();
    public bool StoredInSeparateFile => BulkDataFlags.HasFlag(BulkDataFlags.StoreInSeparateFile);
}