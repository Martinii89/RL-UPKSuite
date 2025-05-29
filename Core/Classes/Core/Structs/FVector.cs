using System.Diagnostics;

namespace RlUpk.Core.Classes.Core.Structs;

[DebuggerDisplay("(X:{X}, Y:{Y}, Z:{Z})")]
public class FVector
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public static FVector Zero = new FVector() { X = 0, Y = 0, Z = 0 };
    public static FVector One = new FVector() { X = 1, Y = 1, Z = 1 };

    public static FVector operator +(FVector a, FVector b)
    {
        return new FVector() { X = a.X + b.X, Y = a.Y + b.Y, Z = a.Z + b.Z };
    }
    
    public static FVector operator *(FVector a, float f)
    {
        return new FVector() { X = a.X * f, Y = a.Y * f, Z = a.Z * f };
    }
}