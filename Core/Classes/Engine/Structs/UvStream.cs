using RlUpk.Core.Classes.Core.Structs;

namespace RlUpk.Core.Classes.Engine.Structs;

public class UvStream
{
    public int NumTexCords { get; set; }
    public int ItemSize { get; set; }
    public int NumVerts { get; set; }
    public int BUseFullPrecisionUVs { get; set; }
    public TArray<UvItem> UvStreamItems { get; set; } = new();
}