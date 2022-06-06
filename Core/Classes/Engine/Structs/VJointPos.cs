using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class VJointPos
{
    public FQuat Orientation { get; set; } = new();
    public FVector Position { get; set; } = new();
}