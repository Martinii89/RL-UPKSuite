using RlUpk.Core.Classes.Core.Structs;
using RlUpk.Core.Types;

namespace RlUpk.Core.Classes.Engine.Structs;

public class FMeshBone
{
    public VJointPos BonePos { get; set; } = new();
    public uint Flags { get; set; }
    public FName Name { get; set; }
    public int NumChildren { get; set; }
    public int ParentIndex { get; set; }
    public FColor BoneColor { get; set; } = new();
}