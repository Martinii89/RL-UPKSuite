using RlUpk.Core.Classes.Core.Structs;

namespace RlUpk.Core.Classes.Engine.Structs;

public class GpuVert
{
    public byte[] BoneIndex { get; set; } = new byte[4];
    public byte[] BoneWeight { get; set; } = new byte[4];
    public uint N0 { get; set; }
    public uint N1 { get; set; }
    public FVector Pos { get; set; } = new();
    public UvHalf[] UV { get; set; } = Array.Empty<UvHalf>();
}