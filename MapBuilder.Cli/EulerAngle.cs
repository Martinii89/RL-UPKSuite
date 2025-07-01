namespace RlUpk.MapBuilder.Cli;

public readonly struct EulerAngles(float x, float y, float z)
{
    public readonly float X = x, Y = y, Z = z;

    public void Deconstruct(out float x, out float y, out float z)
    {
        x = X;
        y = Y;
        z = Z;
    }
}

