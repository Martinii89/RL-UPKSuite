using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class UvStream
{
    public int NumTexCords { get; set; }
    public int ItemSize { get; set; }
    public int NumVerts { get; set; }
    public int BUseFullPrecisionUVs { get; set; }
    public TArray<UvItem> UvStreamItems { get; set; } = new();

    //public void Deserialize(BinaryReader Reader)
    //{
    //    NumTexCords = Reader.ReadInt32();
    //    ItemSize = Reader.ReadInt32();
    //    NumVerts = Reader.ReadInt32();
    //    BUseFullPrecisionUVs = Reader.ReadInt32();
    //    UvStreamItems = new TArrayWithElemtSize<UvItem>(() => new UvItem(new UvHalf[NumTexCords]));
    //    UvStreamItems.Deserialize(Reader);
    //}

    //public void Serialize(IUnrealStream writer)
    //{
    //    writer.Write(NumTexCords);
    //    writer.Write(ItemSize);
    //    writer.Write(NumVerts);
    //    writer.Write(BUseFullPrecisionUVs);
    //    UvStreamItems.Serialize(writer);
    //}
}