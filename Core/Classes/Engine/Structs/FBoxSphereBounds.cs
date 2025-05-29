using RlUpk.Core.Classes.Core.Structs;

namespace RlUpk.Core.Classes.Engine.Structs;

public class FBoxSphereBounds
{
    public FVector Origin { get; set; } = new();
    public FVector BoxExtent { get; set; } = new();
    public float SphereRadius { get; set; }
}