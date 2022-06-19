using Core.Classes.Core.Structs;

namespace Core.Classes.Engine.Structs;

public class FBoxSphereBounds
{
    public FVector Origin { get; set; } = new();
    public FVector BoxExtent { get; set; } = new();
    public float SphereRadius { get; set; }
}