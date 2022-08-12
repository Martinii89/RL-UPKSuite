namespace Core.Classes.Engine.Structs;

public class FSkelMeshChunk
{
    public List<short> Bones { get; set; } = new();
    public int FirstVertex { get; set; }
    public int MaxInfluences { get; set; }
    public int NumRigidVerts { get; set; }
    public int NumSoftVerts { get; set; }
    public List<FRigidVertex> RigidVerts { get; set; } = new();
    public List<FSoftVertex> SoftVerts { get; set; } = new();
}