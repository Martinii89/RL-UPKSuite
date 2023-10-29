namespace Core.Classes.Engine.Structs;

public class UvItem
{
    public UvItem()
    {
    }

    public UvItem(UvHalf[] uv)
    {
        Uv = uv;
    }

    public UvItem(UvFull[] uv)
    {
        UvFull = uv;
    }

    public uint N0 { get; set; }
    public uint N1 { get; set; }
    public UvHalf[]? Uv { get; }
    public UvFull[]? UvFull { get; }
}