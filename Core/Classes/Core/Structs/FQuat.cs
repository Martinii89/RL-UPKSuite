namespace Core.Classes.Core.Structs;

public class FQuat
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }
}

public class FMatrix
{
    public float[] M { get; set; } = new float[16];
}