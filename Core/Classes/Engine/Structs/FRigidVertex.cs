using RlUpk.Core.Classes.Core.Structs;

namespace RlUpk.Core.Classes.Engine.Structs;

public class FRigidVertex
{
    public byte BoneIndex { get; set; }
    public FColor Color { get; set; } = new();
    public uint[] Normal { get; set; } = new uint[3];
    public FVector Pos { get; set; } = new();
    public FVector2D[] UV { get; set; } = new FVector2D[4];
}