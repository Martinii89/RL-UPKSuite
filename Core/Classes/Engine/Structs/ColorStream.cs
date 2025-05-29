using RlUpk.Core.Classes.Core.Structs;

namespace RlUpk.Core.Classes.Engine.Structs;

public class ColorStream
{
    public int ItemSize { get; set; }
    public int NumVerts { get; set; }
    public TArray<FColor> Colors { get; set; } = new();
}