using RlUpk.Core.Classes.Core.Structs;

namespace RlUpk.Core.Classes.Engine.Structs;

public class VJointPos
{
    public FQuat Orientation { get; set; } = new();
    public FVector Position { get; set; } = new();
}