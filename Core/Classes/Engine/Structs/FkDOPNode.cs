namespace Core.Classes.Engine.Structs;

public class FkDOPNode
{
    public byte[] Mins { get; init; } = new byte[3];
    public byte[] Maxs { get; init; } = new byte[3];
}