using RlUpk.Core.Classes.Core.Structs;

namespace RlUpk.Core.Classes.Engine.Structs;

public class FSkelIndexBuffer
{
    public TArray<uint> Indices { get; set; } = new();
    public byte Size { get; set; }
    public int Unk { get; set; }
}